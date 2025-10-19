using UniversityMS.Domain.Entities.Common;
using UniversityMS.Domain.Enums;
using UniversityMS.Domain.Exceptions;
using UniversityMS.Domain.ValueObjects;

namespace UniversityMS.Domain.Entities.PayrollAggregate;

/// <summary>
/// Payslip (Bordro Raporu / Maaş Fişi)
/// Çalışana verilen aylık maaş fişi. PDF dosya olarak oluşturulur ve arşivlenir.
/// </summary>
public class Payslip : AuditableEntity
{
    public Guid PayrollId { get; private set; }
    public Guid EmployeeId { get; private set; }
    public string PayrollNumber { get; private set; } = null!;

    public int Year { get; private set; }
    public int Month { get; private set; }
    public string Period { get; private set; } = null!; // "10/2025" format

    // Dosya Bilgileri
    public string? FilePath { get; private set; }
    public string? FileName { get; private set; }
    public long? FileSize { get; private set; } // Byte cinsinden
    public string? FileHash { get; private set; } // SHA256 hash

    // İçerik Özeti (denormalize edilmiş - performans için)
    public Money GrossSalary { get; private set; } = null!;
    public Money TotalDeductions { get; private set; } = null!;
    public Money NetSalary { get; private set; } = null!;

    // İş Günü Bilgileri
    public int WorkingDays { get; private set; }
    public int ActualWorkDays { get; private set; }
    public decimal OvertimeHours { get; private set; }
    public int LeaveDays { get; private set; }
    public int AbsentDays { get; private set; }

    // Vergi Bilgileri (Türkiye)
    public Money IncomeTax { get; private set; } = null!;
    public Money SGKEmployeeContribution { get; private set; } = null!;
    public Money SGKEmployerContribution { get; private set; } = null!;

    // Ödeme Bilgileri
    public PaymentMethod PaymentMethod { get; private set; }
    public string? BankAccount { get; private set; }
    public string? BankName { get; private set; }
    public string? IBAN { get; private set; }

    // Durum ve Tarihler
    public PayslipStatus Status { get; private set; }
    public DateTime GeneratedDate { get; private set; }
    public DateTime? PrintedDate { get; private set; }
    public DateTime? EmailSentDate { get; private set; }
    public DateTime? DownloadedDate { get; private set; }

    // İmza ve Onay
    public string? DigitalSignature { get; private set; } // E-imza
    public bool IsApproved { get; private set; }
    public DateTime? ApprovedDate { get; private set; }
    public Guid? ApprovedBy { get; private set; }

    public string? Notes { get; private set; }

    // Navigation Properties
    public Payroll Payroll { get; private set; } = null!;

    // Parameterless constructor for EF Core
    private Payslip() { }

    private Payslip(
        Guid payrollId,
        Guid employeeId,
        string payrollNumber,
        int year,
        int month,
        Money grossSalary,
        Money totalDeductions,
        Money netSalary,
        int workingDays,
        int actualWorkDays,
        PaymentMethod paymentMethod)
    {
        PayrollId = payrollId;
        EmployeeId = employeeId;
        PayrollNumber = payrollNumber;
        Year = year;
        Month = month;
        Period = $"{month:D2}/{year}";

        GrossSalary = grossSalary;
        TotalDeductions = totalDeductions;
        NetSalary = netSalary;

        WorkingDays = workingDays;
        ActualWorkDays = actualWorkDays;
        PaymentMethod = paymentMethod;

        Status = PayslipStatus.Draft;
        GeneratedDate = DateTime.UtcNow;
        IsApproved = false;
    }

    /// <summary>
    /// Yeni Payslip oluştur
    /// </summary>
    public static Payslip Create(
        Guid payrollId,
        Guid employeeId,
        string payrollNumber,
        int year,
        int month,
        Money grossSalary,
        Money totalDeductions,
        Money netSalary,
        int workingDays,
        int actualWorkDays,
        PaymentMethod paymentMethod)
    {
        if (year < 2000 || year > 2100)
            throw new DomainException("Geçerli bir yıl giriniz.");

        if (month < 1 || month > 12)
            throw new DomainException("Ay 1-12 arasında olmalıdır.");

        if (workingDays <= 0)
            throw new DomainException("Çalışma günü sayısı sıfırdan büyük olmalıdır.");

        return new Payslip(
            payrollId,
            employeeId,
            payrollNumber,
            year,
            month,
            grossSalary,
            totalDeductions,
            netSalary,
            workingDays,
            actualWorkDays,
            paymentMethod);
    }

    /// <summary>
    /// Payslip bilgilerini güncelle (Bordro verilerine göre)
    /// </summary>
    public void UpdateFromPayroll(
        Money grossSalary,
        Money totalDeductions,
        Money netSalary,
        Money incomeTax,
        Money sgkEmployeeContribution,
        Money sgkEmployerContribution,
        int workingDays,
        int actualWorkDays,
        int leaveDays,
        int absentDays,
        decimal overtimeHours)
    {
        if (Status != PayslipStatus.Draft && Status != PayslipStatus.Generated)
            throw new DomainException("Payslip güncellenemez. Durum: " + Status);

        GrossSalary = grossSalary;
        TotalDeductions = totalDeductions;
        NetSalary = netSalary;
        IncomeTax = incomeTax;
        SGKEmployeeContribution = sgkEmployeeContribution;
        SGKEmployerContribution = sgkEmployerContribution;

        WorkingDays = workingDays;
        ActualWorkDays = actualWorkDays;
        LeaveDays = leaveDays;
        AbsentDays = absentDays;
        OvertimeHours = overtimeHours;
    }

    /// <summary>
    /// PDF dosya bilgisini kaydet
    /// </summary>
    public void SetPdfFile(string filePath, string fileName, long fileSize, string fileHash)
    {
        if (string.IsNullOrWhiteSpace(filePath))
            throw new DomainException("Dosya yolu boş olamaz.");

        FilePath = filePath;
        FileName = fileName;
        FileSize = fileSize;
        FileHash = fileHash;
        Status = PayslipStatus.Generated;
    }

    /// <summary>
    /// Banka bilgisini kaydet
    /// </summary>
    public void SetBankInfo(string? bankName, string? iban, string? accountNumber)
    {
        BankName = bankName;
        IBAN = iban;
        BankAccount = accountNumber;
    }

    /// <summary>
    /// PDF yazdırma kaydı
    /// </summary>
    public void MarkAsPrinted()
    {
        if (Status != PayslipStatus.Generated)
            throw new DomainException("Sadece oluşturulmuş payslip'ler yazdırılabilir.");

        PrintedDate = DateTime.UtcNow;
        Status = PayslipStatus.Printed;
    }

    /// <summary>
    /// Email gönderim kaydı
    /// </summary>
    public void MarkAsEmailSent()
    {
        if (Status != PayslipStatus.Generated && Status != PayslipStatus.Printed)
            throw new DomainException("Payslip henüz hazır değil.");

        EmailSentDate = DateTime.UtcNow;
        Status = PayslipStatus.EmailSent;
    }

    /// <summary>
    /// İndirme kaydı
    /// </summary>
    public void MarkAsDownloaded()
    {
        DownloadedDate = DateTime.UtcNow;
        Status = PayslipStatus.Downloaded;
    }

    /// <summary>
    /// E-imza ekle
    /// </summary>
    public void SetDigitalSignature(string signature)
    {
        if (string.IsNullOrWhiteSpace(signature))
            throw new DomainException("İmza boş olamaz.");

        DigitalSignature = signature;
        Status = PayslipStatus.Signed;
    }

    /// <summary>
    /// Payslip'i onayla
    /// </summary>
    public void Approve(Guid approverUserId)
    {
        IsApproved = true;
        ApprovedDate = DateTime.UtcNow;
        ApprovedBy = approverUserId;
        Status = PayslipStatus.Approved;
    }

    /// <summary>
    /// Payslip'i arşivle
    /// </summary>
    public void Archive()
    {
        if (Status == PayslipStatus.Archived)
            throw new DomainException("Payslip zaten arşivlenmiş.");

        Status = PayslipStatus.Archived;
    }
}