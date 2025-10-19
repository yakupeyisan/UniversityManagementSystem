using iText.IO.Font;
using iText.IO.Font.Constants;
using iText.Kernel.Font;
using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Element;
using iText.Layout.Properties;
using Microsoft.Extensions.Logging;
using System.Text;
using UniversityMS.Application.Common.Interfaces;
using UniversityMS.Domain.Entities.HRAggregate;
using UniversityMS.Domain.Entities.PayrollAggregate;
using UniversityMS.Domain.Enums;
using UniversityMS.Domain.Interfaces;
using UniversityMS.Domain.ValueObjects;
using static iText.IO.Font.FontProgram;

namespace UniversityMS.Infrastructure.Services;

public class PayslipGenerationService : IPayslipGenerationService
{
    private readonly IRepository<Payslip> _payslipRepository;
    private readonly IFileStorageService _fileStorage;
    private readonly IEmailService _emailService;
    private readonly ILogger<PayslipGenerationService> _logger;

    public PayslipGenerationService(
        IRepository<Payslip> payslipRepository,
        IFileStorageService fileStorage,
        IEmailService emailService,
        ILogger<PayslipGenerationService> logger)
    {
        _payslipRepository = payslipRepository;
        _fileStorage = fileStorage;
        _emailService = emailService;
        _logger = logger;
    }

    public async Task<Payslip> GeneratePayslipAsync(
        Payroll payroll,
        Employee employee,
        CancellationToken cancellationToken)
    {
        try
        {
            // Payslip entity oluştur
            var payslip = Payslip.Create(
                payroll.Id,
                payroll.EmployeeId,
                payroll.PayrollNumber,
                payroll.Year,
                payroll.Month,
                payroll.TotalEarnings,
                payroll.TotalDeductions,
                payroll.NetSalary,
                payroll.WorkingDays,
                payroll.ActualWorkDays,
                payroll.PaymentMethod);

            // Vergi bilgilerini ayarla
            payslip.UpdateFromPayroll(
                payroll.TotalEarnings,
                payroll.TotalDeductions,
                payroll.NetSalary,
                Money.Create(0, "TRY"), // İncome Tax - bordrodan alınacak
                Money.Create(0, "TRY"), // SGK Employee
                Money.Create(0, "TRY"), // SGK Employer
                payroll.WorkingDays,
                payroll.ActualWorkDays,
                payroll.LeaveDays,
                payroll.AbsentDays,
                payroll.OvertimeHours);

            // Banka bilgilerini ayarla - BankInfo olmadığı için kontrol kaldırıldı
            // Gerekirse Employee entity'sine BankInfo property'si eklenebilir

            // PDF oluştur
            var pdfPath = await GeneratePdfAsync(payroll, employee, cancellationToken);

            if (!string.IsNullOrEmpty(pdfPath))
            {
                var fileInfo = new FileInfo(pdfPath);
                var fileHash = ComputeFileHash(pdfPath);

                payslip.SetPdfFile(
                    pdfPath,
                    fileInfo.Name,
                    fileInfo.Length,
                    fileHash);
            }

            // Database'e kaydet
            await _payslipRepository.AddAsync(payslip, cancellationToken);

            _logger.LogInformation(
                "Payslip oluşturuldu: {PayrollNumber} - Çalışan: {EmployeeId}",
                payroll.PayrollNumber,
                payroll.EmployeeId);

            return payslip;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Payslip oluşturma hatası: {PayrollNumber}", payroll.PayrollNumber);
            throw;
        }
    }

    public async Task<string> GeneratePdfAsync(
        Payroll payroll,
        Employee employee,
        CancellationToken cancellationToken)
    {
        try
        {
            var fileName = $"Payslip_{payroll.EmployeeId}_{payroll.Year}_{payroll.Month:D2}.pdf";
            var filePath = Path.Combine("payslips", payroll.Year.ToString(), payroll.Month.ToString("D2"));

            // PDF dosyası oluştur (iText7 kullanarak)
            var pdfBytes = CreatePayslipPdf(payroll, employee);

            // Dosyayı depolama servisiyle kaydet
            var savedPath = await _fileStorage.SaveFileAsync(
                pdfBytes,
                fileName,
                filePath,
                "application/pdf",
                cancellationToken);

            _logger.LogInformation("Payslip PDF oluşturuldu: {FilePath}", savedPath);
            return savedPath;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "PDF oluşturma hatası");
            throw;
        }
    }

    public async Task<bool> SendPayslipEmailAsync(
        Payslip payslip,
        Employee employee,
        CancellationToken cancellationToken)
    {
        try
        {
            if (string.IsNullOrEmpty(payslip.FilePath))
                throw new InvalidOperationException("Payslip dosyası bulunamadı.");

            var subject = $"Maaş Fişi - {payslip.Period}";
            var body = GenerateEmailBody(payslip, employee);

            var result = await _emailService.SendEmailWithAttachmentAsync(
                employee.Person.Email,
                subject,
                body,
                payslip.FilePath,
                cancellationToken);

            if (result)
            {
                payslip.MarkAsEmailSent();
                _logger.LogInformation("Payslip email gönderildi: {EmployeeEmail}", employee.Person.Email);
            }

            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Email gönderme hatası");
            return false;
        }
    }

    private byte[] CreatePayslipPdf(Payroll payroll, Employee employee)
    {
        using (var ms = new MemoryStream())
        {
            var writer = new PdfWriter(ms);
            var pdf = new PdfDocument(writer);
            var document = new Document(pdf);

            // Başlık
            var titleFont = PdfFontFactory.CreateFont(StandardFonts.HELVETICA_BOLD);
            document.Add(new Paragraph("AYLIK MAAŞ FİŞİ")
                .SetFont(titleFont)
                .SetFontSize(18)
                .SetTextAlignment(TextAlignment.CENTER));

            document.Add(new Paragraph($"Dönem: {payroll.Month}/{payroll.Year}")
                .SetFontSize(12)
                .SetTextAlignment(TextAlignment.CENTER));

            // Çalışan Bilgileri
            var boldFont = PdfFontFactory.CreateFont(StandardFonts.HELVETICA_BOLD);
            document.Add(new Paragraph("\nÇALIŞAN BİLGİLERİ")
                .SetFont(boldFont)
                .SetFontSize(12));

            var employeeTable = new Table(new float[] { 1, 1 });
            employeeTable.AddCell("Adı Soyadı");
            employeeTable.AddCell($"{employee.Person.FirstName} {employee.Person.LastName}");
            employeeTable.AddCell("Çalışan No");
            employeeTable.AddCell(employee.EmployeeNumber.Value);
            employeeTable.AddCell("Pozisyon");
            employeeTable.AddCell(employee.JobTitle);
            employeeTable.AddCell("Departman");
            employeeTable.AddCell(employee.Department?.Name ?? "N/A");
            document.Add(employeeTable);

            // Maaş Detayları
            document.Add(new Paragraph("\nMAAAŞ DETAYLARı")
                .SetFont(boldFont)
                .SetFontSize(12));

            var salaryTable = new Table(new float[] { 1, 1 });
            salaryTable.AddCell("Temel Maaş");
            salaryTable.AddCell($"₺ {payroll.BaseSalary.Amount:N2}");
            salaryTable.AddCell("Toplam Kazançlar");
            salaryTable.AddCell($"₺ {payroll.TotalEarnings.Amount:N2}");
            salaryTable.AddCell("Toplam Kesintiler");
            salaryTable.AddCell($"₺ {payroll.TotalDeductions.Amount:N2}");
            salaryTable.AddCell("NET MAAŞ");
            salaryTable.AddCell($"₺ {payroll.NetSalary.Amount:N2}");
            document.Add(salaryTable);

            // İş Günü Bilgileri
            document.Add(new Paragraph("\nİŞ GÜNÜ BİLGİLERİ")
                .SetFont(boldFont)
                .SetFontSize(12));

            var workTable = new Table(new float[] { 1, 1 });
            workTable.AddCell("Çalışma Günü");
            workTable.AddCell(payroll.WorkingDays.ToString());
            workTable.AddCell("Gerçek Çalışma Günü");
            workTable.AddCell(payroll.ActualWorkDays.ToString());
            workTable.AddCell("İzin Günü");
            workTable.AddCell(payroll.LeaveDays.ToString());
            workTable.AddCell("Devamsızlık");
            workTable.AddCell(payroll.AbsentDays.ToString());
            workTable.AddCell("Fazla Mesai Saati");
            workTable.AddCell($"{payroll.OvertimeHours:N1}");
            document.Add(workTable);

            // Ödeme Bilgileri
            if (payroll.PaymentMethod == PaymentMethod.BankTransfer)
            {
                document.Add(new Paragraph("\nÖDEME BİLGİLERİ")
                    .SetFont(boldFont)
                    .SetFontSize(12));

                var paymentTable = new Table(new float[] { 1, 1 });
                paymentTable.AddCell("Ödeme Yöntemi");
                paymentTable.AddCell("Banka Havalesi");
                paymentTable.AddCell("Ödeme Tarihi");
                paymentTable.AddCell(payroll.PaymentDate?.ToString("dd.MM.yyyy") ?? "-");
                document.Add(paymentTable);
            }

            document.Close();
            return ms.ToArray();
        }
    }

    private string GenerateEmailBody(Payslip payslip, Employee employee)
    {
        var sb = new StringBuilder();
        sb.AppendLine($"Sayın {employee.Person.FirstName} {employee.Person.LastName},");
        sb.AppendLine();
        sb.AppendLine($"{payslip.Period} dönemine ait maaş fişiniz ektedir.");
        sb.AppendLine();
        sb.AppendLine($"Brüt Maaş: ₺ {payslip.GrossSalary.Amount:N2}");
        sb.AppendLine($"Toplam Kesintiler: ₺ {payslip.TotalDeductions.Amount:N2}");
        sb.AppendLine($"Net Maaş: ₺ {payslip.NetSalary.Amount:N2}");
        sb.AppendLine();
        sb.AppendLine("Saygılarımızla,");
        sb.AppendLine("İnsan Kaynakları Departmanı");

        return sb.ToString();
    }

    private string ComputeFileHash(string filePath)
    {
        using (var sha256 = System.Security.Cryptography.SHA256.Create())
        using (var fileStream = File.OpenRead(filePath))
        {
            var hashBytes = sha256.ComputeHash(fileStream);
            return Convert.ToBase64String(hashBytes);
        }
    }
}