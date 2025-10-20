using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using UniversityMS.Application.Common.Models;
using UniversityMS.Application.Features.PayrollFeature.DTOs;
using UniversityMS.Domain.Entities.PayrollAggregate;
using UniversityMS.Domain.Interfaces;

namespace UniversityMS.Application.Features.PayrollFeature.Queries;

/// <summary>
/// Payslip Detayını ID'ye Göre Getir Handler
/// </summary>
public class GetPayslipByIdQueryHandler : IRequestHandler<GetPayslipByIdQuery, Result<PayslipDetailDto>>
{
    private readonly IRepository<Payslip> _payslipRepository;
    private readonly IRepository<Payroll> _payrollRepository;
    private readonly IMapper _mapper;
    private readonly ILogger<GetPayslipByIdQueryHandler> _logger;

    public GetPayslipByIdQueryHandler(
        IRepository<Payslip> payslipRepository,
        IRepository<Payroll> payrollRepository,
        IMapper mapper,
        ILogger<GetPayslipByIdQueryHandler> logger)
    {
        _payslipRepository = payslipRepository;
        _payrollRepository = payrollRepository;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<Result<PayslipDetailDto>> Handle(
        GetPayslipByIdQuery request,
        CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("Payslip detayı getiriliyor. PayslipId: {PayslipId}", request.PayslipId);

            // ========== PAYSLIP BULMA ==========
            var payslip = await _payslipRepository.GetByIdAsync(request.PayslipId, cancellationToken);

            if (payslip == null)
            {
                _logger.LogWarning("Payslip bulunamadı. PayslipId: {PayslipId}", request.PayslipId);
                return Result<PayslipDetailDto>.Failure("Payslip bulunamadı.");
            }

            // ========== BORDRO BİLGİLERİ ALMA ==========
            var payroll = await _payrollRepository.GetByIdAsync(payslip.PayrollId, cancellationToken);

            if (payroll == null)
            {
                _logger.LogWarning("Payslip'e ait bordro bulunamadı. PayslipId: {PayslipId}", request.PayslipId);
                return Result<PayslipDetailDto>.Failure("Payslip'e ait bordro bulunamadı.");
            }

            // ========== DTO HAZIRLAMA ==========
            var dto = new PayslipDetailDto
            {
                // Temel Bilgiler
                PayslipId = payslip.Id,
                PayrollNumber = payslip.PayrollNumber,
                Period = payslip.Period,

                // Çalışan Bilgileri
                EmployeeFullName = payroll.Employee?.Person?.FirstName + " " + payroll.Employee?.Person?.LastName ?? "N/A",
                EmployeeNumber = payroll.Employee?.EmployeeNumber?.Value ?? "N/A",
                Designation = payroll.Employee?.JobTitle ?? "N/A",
                Department = payroll.Employee?.Department?.Name ?? "N/A",

                // Dönem Bilgileri
                GeneratedDate = payslip.GeneratedDate,
                PaymentDate = payroll.PaymentDate,
                PaymentMethod = payroll.PaymentMethod.ToString(),

                // Maaş Bilgileri
                BaseSalary = payroll.BaseSalary.Amount,
                GrossSalary = payslip.GrossSalary.Amount,
                TotalDeductions = payslip.TotalDeductions.Amount,
                NetSalary = payslip.NetSalary.Amount,

                // Kazançlar (Bordro kalemlerinden)
                Earnings = payroll.Items?
                    .Where(i => i.Type.ToString() == "Earning")
                    .Select(i => new PayslipLineItemDto
                    {
                        Id = i.Id,
                        Type = "Earning",
                        Category = i.Category,
                        Description = i.Description,
                        Amount = i.Amount.Amount,
                        Quantity = i.Quantity,
                        IsTaxable = i.IsTaxable
                    })
                    .ToList() ?? new(),

                // Kesintiler (Bordro kalemlerinden)
                Deductions = payroll.Deductions?
                    .Select(d => new PayslipLineItemDto
                    {
                        Id = d.Id,
                        Type = "Deduction",
                        Category = d.Type.ToString(),
                        Description = d.Description,
                        Amount = d.Amount.Amount,
                        Rate = d.Rate,
                        IsStatutory = d.IsStatutory,
                        Reference = d.Reference
                    })
                    .ToList() ?? new(),

                // TÜRKİYE VERGİ BİLGİLERİ
                IncomeTax = payslip.IncomeTax.Amount,
                SGKEmployeeContribution = payslip.SGKEmployeeContribution.Amount,
                SGKEmployerContribution = payslip.SGKEmployerContribution.Amount,
                UnemploymentInsuranceEmployee = payslip.SGKEmployeeContribution.Amount * 0.01m / 0.14m, // Yaklaşık hesap

                // ÇALIŞMA GÜN/SAATLERİ
                WorkingDays = payslip.WorkingDays,
                ActualWorkDays = payslip.ActualWorkDays,
                AbsentDays = payslip.AbsentDays,
                LeaveDays = payslip.LeaveDays,
                OvertimeHours = payslip.OvertimeHours,

                // ÖDEME BİLGİLERİ
                BankName = payslip.BankName,
                IBAN = payslip.IBAN,
                BankAccount = payslip.BankAccount,

                // IMZA VE ONAY
                IsApproved = payslip.IsApproved,
                ApprovedDate = payslip.ApprovedDate,

                // NOTLAR
                Notes = payslip.Notes
            };

            _logger.LogInformation("Payslip detayı başarıyla hazırlandı. PayslipId: {PayslipId}", request.PayslipId);

            return Result<PayslipDetailDto>.Success(dto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Payslip detayı getirme hatası. PayslipId: {PayslipId}", request.PayslipId);
            return Result<PayslipDetailDto>.Failure($"Hata: {ex.Message}");
        }
    }
}