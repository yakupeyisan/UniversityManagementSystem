using MediatR;
using Microsoft.Extensions.Logging;
using UniversityMS.Application.Common.Models;
using UniversityMS.Application.Features.PayrollFeature.DTOs;
using UniversityMS.Domain.Entities.PayrollAggregate;
using UniversityMS.Domain.Interfaces;

namespace UniversityMS.Application.Features.PayrollFeature.Commands;

/// <summary>
/// SGK Hesaplama Handler
/// Türkiye SGK (Sosyal Güvenlik Kurumu) primlerini hesaplar
/// </summary>
public class CalculateSGKCommandHandler : IRequestHandler<CalculateSGKCommand, Result<SGKCalculationResultDto>>
{
    private readonly IRepository<Payroll> _payrollRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<CalculateSGKCommandHandler> _logger;

    public CalculateSGKCommandHandler(
        IRepository<Payroll> payrollRepository,
        IUnitOfWork unitOfWork,
        ILogger<CalculateSGKCommandHandler> logger)
    {
        _payrollRepository = payrollRepository;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Result<SGKCalculationResultDto>> Handle(
        CalculateSGKCommand request,
        CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation(
                "SGK hesaplama başlıyor. PayrollId: {PayrollId}, Brüt: {Gross}",
                request.PayrollId, request.GrossSalary);

            // ========== VALIDASYON ==========
            if (request.GrossSalary <= 0)
                return Result<SGKCalculationResultDto>.Failure("Brüt maaş sıfırdan büyük olmalıdır.");

            if (request.PremiumDays <= 0 || request.PremiumDays > 31)
                return Result<SGKCalculationResultDto>.Failure("Prim günü 1-31 arasında olmalıdır.");

            // ========== VERİ HAZIRLAMA ==========
            var calculationYear = request.CalculationYear ?? DateTime.UtcNow.Year;
            var calculationMonth = request.CalculationMonth ?? DateTime.UtcNow.Month;
            var grossSalary = request.GrossSalary;

            // ========== MUAFIYET KONTROLÜ ==========
            if (request.IsExempt)
            {
                _logger.LogInformation("SGK muafiyeti uygulanıyor. PayrollId: {PayrollId}", request.PayrollId);

                return Result<SGKCalculationResultDto>.Success(new SGKCalculationResultDto
                {
                    GrossSalary = grossSalary,
                    SGKCalculationBasis = 0,
                    EmployeeContributionRate = 0,
                    EmployeeContributionAmount = 0,
                    EmployerContributionRate = 0,
                    EmployerContributionAmount = 0,
                    UnemploymentInsuranceEmployeeRate = 0,
                    UnemploymentInsuranceEmployeeAmount = 0,
                    UnemploymentInsuranceEmployerRate = 0,
                    UnemploymentInsuranceEmployerAmount = 0,
                    TotalEmployeeContribution = 0,
                    TotalEmployerContribution = 0,
                    TotalEmployerCost = grossSalary,
                    IsInsured = false,
                    PremiumDays = request.PremiumDays,
                    IsExempt = true,
                    ExemptionReason = "İdari karar ile muafiyetli",
                    CalculationPeriod = $"{calculationMonth:D2}/{calculationYear}",
                    SGKTariffVersion = "2025-Turkey-v1",
                    CalculatedDate = DateTime.UtcNow,
                    CalculatedBy = "PayrollSystem",
                    CalculationNotes = "SGK muafiyeti uygulandı"
                });
            }

            // ========== 2025 SGK ORANI (TÜRKIYE) ==========
            // SGK Çalışan Prim Payı: %14
            // SGK İşveren Prim Payı: %22.2
            // İşsizlik Sigortası Çalışan: %1
            // İşsizlik Sigortası İşveren: %2

            decimal sgkCalculationBasis = grossSalary;

            // ========== SGK ÇALIŞAN PAYI (%) ==========
            decimal employeeContributionRate = 14m; // %14
            decimal employeeContributionAmount = sgkCalculationBasis * (employeeContributionRate / 100m);

            // ========== SGK İŞVEREN PAYI (%) ==========
            decimal employerContributionRate = 22.2m; // %22.2
            decimal employerContributionAmount = sgkCalculationBasis * (employerContributionRate / 100m);

            // ========== İŞSİZLİK SİGORTASI ÇALIŞAN PAYI (%1) ==========
            decimal unemploymentInsuranceEmployeeRate = 1m; // %1
            decimal unemploymentInsuranceEmployeeAmount = sgkCalculationBasis * (unemploymentInsuranceEmployeeRate / 100m);

            // ========== İŞSİZLİK SİGORTASI İŞVEREN PAYI (%2) ==========
            decimal unemploymentInsuranceEmployerRate = 2m; // %2
            decimal unemploymentInsuranceEmployerAmount = sgkCalculationBasis * (unemploymentInsuranceEmployerRate / 100m);

            // ========== TOPLAM KESİNTİLER ==========
            decimal totalEmployeeContribution = employeeContributionAmount + unemploymentInsuranceEmployeeAmount;
            decimal totalEmployerContribution = employerContributionAmount + unemploymentInsuranceEmployerAmount;
            decimal totalEmployerCost = grossSalary + totalEmployerContribution;

            // ========== SONUÇ HAZIRLAMA ==========
            var result = new SGKCalculationResultDto
            {
                GrossSalary = grossSalary,
                SGKCalculationBasis = sgkCalculationBasis,
                EmployeeContributionRate = employeeContributionRate,
                EmployeeContributionAmount = employeeContributionAmount,
                EmployerContributionRate = employerContributionRate,
                EmployerContributionAmount = employerContributionAmount,
                UnemploymentInsuranceEmployeeRate = unemploymentInsuranceEmployeeRate,
                UnemploymentInsuranceEmployeeAmount = unemploymentInsuranceEmployeeAmount,
                UnemploymentInsuranceEmployerRate = unemploymentInsuranceEmployerRate,
                UnemploymentInsuranceEmployerAmount = unemploymentInsuranceEmployerAmount,
                TotalEmployeeContribution = totalEmployeeContribution,
                EmployeeContributionBreakdown = $"SGK: {employeeContributionAmount:N2} TL + İşsizlik: {unemploymentInsuranceEmployeeAmount:N2} TL",
                TotalEmployerContribution = totalEmployerContribution,
                EmployerContributionBreakdown = $"SGK: {employerContributionAmount:N2} TL + İşsizlik: {unemploymentInsuranceEmployerAmount:N2} TL",
                TotalEmployerCost = totalEmployerCost,
                IsInsured = request.IsInsured,
                PremiumDays = request.PremiumDays,
                IsExempt = false,
                CalculationPeriod = $"{calculationMonth:D2}/{calculationYear}",
                SGKTariffVersion = "2025-Turkey-v1",
                CalculatedDate = DateTime.UtcNow,
                CalculatedBy = "PayrollSystem",
                CalculationNotes = $"SGK hesaplama dönemi: {calculationMonth}/{calculationYear}, Prim günü: {request.PremiumDays}",
                LegalReferences = "SGK Kanunu, 506 Sayılı Kanun - Gümrük ve Ticaret Bakanlığı 2025"
            };

            _logger.LogInformation(
                "SGK hesaplama tamamlandı. PayrollId: {PayrollId}, Çalışan: {Employee}, İşveren: {Employer}",
                request.PayrollId, totalEmployeeContribution, totalEmployerContribution);

            return Result<SGKCalculationResultDto>.Success(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "SGK hesaplama hatası. PayrollId: {PayrollId}", request.PayrollId);
            return Result<SGKCalculationResultDto>.Failure($"SGK hesaplama hatası: {ex.Message}");
        }
    }
}