using Microsoft.Extensions.Logging;
using UniversityMS.Application.Common.Interfaces;
using UniversityMS.Application.Features.PayrollFeature.DTOs;

namespace UniversityMS.Infrastructure.Services;

/// <summary>
/// SGK Hesaplama Servisi Implementation
/// Türkiye SGK primlerini hesaplar
/// 2025 Türkiye SGK Tarifesi
/// </summary>
public class SGKCalculationService : ISGKCalculationService
{
    private readonly ILogger<SGKCalculationService> _logger;

    // SGK Prim Oranları (2025)
    private const decimal EMPLOYEE_SGK_RATE = 0.14m; // %14
    private const decimal EMPLOYER_SGK_RATE = 0.222m; // %22.2
    private const decimal EMPLOYEE_UNEMPLOYMENT_RATE = 0.01m; // %1
    private const decimal EMPLOYER_UNEMPLOYMENT_RATE = 0.02m; // %2

    public SGKCalculationService(ILogger<SGKCalculationService> logger)
    {
        _logger = logger;
    }

    public async Task<SGKCalculationResultDto> CalculateSGKAsync(
        decimal grossSalary,
        int premiumDays = 30,
        bool isInsured = true)
    {
        try
        {
            _logger.LogInformation(
                "SGK hesaplama başlıyor. Brüt: {Gross}, Prim Günü: {Days}",
                grossSalary, premiumDays);

            // ========== VALIDASYON ==========
            if (grossSalary <= 0)
                throw new ArgumentException("Brüt maaş sıfırdan büyük olmalıdır.");

            if (premiumDays < 1 || premiumDays > 31)
                throw new ArgumentException("Prim günü 1-31 arasında olmalıdır.");

            // ========== SGK HESAPLAMA MATRAHÄ° ==========
            // Tam 30 gün için hesaplama yapılır
            decimal sgkCalculationBasis = (grossSalary / 30m) * premiumDays;

            _logger.LogInformation(
                "SGK Matrahı: {Basis} (Gün: {Days})",
                sgkCalculationBasis, premiumDays);

            // ========== MUAFIYET KONTROLÜ ==========
            if (!isInsured)
            {
                _logger.LogInformation("SGK muafiyeti uygulanıyor.");
                return CreateExemptResult(grossSalary);
            }

            // ========== ÇALIŞıAN SGK PAYı (%14) ==========
            decimal employeeContribution = sgkCalculationBasis * EMPLOYEE_SGK_RATE;

            // ========== İŞVEREN SGK PAYı (%22.2) ==========
            decimal employerContribution = sgkCalculationBasis * EMPLOYER_SGK_RATE;

            // ========== İŞSİZLİK SİGORTASI ÇALIŞAN PAYı (%1) ==========
            decimal unemploymentEmployeeAmount = sgkCalculationBasis * EMPLOYEE_UNEMPLOYMENT_RATE;

            // ========== İŞSİZLİK SİGORTASI İŞVEREN PAYı (%2) ==========
            decimal unemploymentEmployerAmount = sgkCalculationBasis * EMPLOYER_UNEMPLOYMENT_RATE;

            // ========== TOPLAM HESAPLAMA ==========
            decimal totalEmployeeContribution = employeeContribution + unemploymentEmployeeAmount;
            decimal totalEmployerContribution = employerContribution + unemploymentEmployerAmount;
            decimal totalEmployerCost = grossSalary + totalEmployerContribution;

            _logger.LogInformation(
                "SGK hesaplama tamamlandı. Çalışan: {Employee}, İşveren: {Employer}, Toplam Maliyet: {Cost}",
                totalEmployeeContribution, totalEmployerContribution, totalEmployerCost);

            // ========== SONUÇ HAZIRLAMA ==========
            var result = new SGKCalculationResultDto
            {
                GrossSalary = grossSalary,
                SGKCalculationBasis = sgkCalculationBasis,
                EmployeeContributionRate = EMPLOYEE_SGK_RATE * 100, // Yüzde olarak
                EmployeeContributionAmount = employeeContribution,
                EmployerContributionRate = EMPLOYER_SGK_RATE * 100,
                EmployerContributionAmount = employerContribution,
                UnemploymentInsuranceEmployeeRate = EMPLOYEE_UNEMPLOYMENT_RATE * 100,
                UnemploymentInsuranceEmployeeAmount = unemploymentEmployeeAmount,
                UnemploymentInsuranceEmployerRate = EMPLOYER_UNEMPLOYMENT_RATE * 100,
                UnemploymentInsuranceEmployerAmount = unemploymentEmployerAmount,
                TotalEmployeeContribution = totalEmployeeContribution,
                EmployeeContributionBreakdown =
                    $"SGK: {employeeContribution:N2} TL + İşsizlik: {unemploymentEmployeeAmount:N2} TL",
                TotalEmployerContribution = totalEmployerContribution,
                EmployerContributionBreakdown =
                    $"SGK: {employerContribution:N2} TL + İşsizlik: {unemploymentEmployerAmount:N2} TL",
                TotalEmployerCost = totalEmployerCost,
                IsInsured = true,
                PremiumDays = premiumDays,
                IsExempt = false,
                CalculationPeriod = $"{DateTime.UtcNow:MM/yyyy}",
                SGKTariffVersion = "2025-Turkey-v1",
                CalculatedDate = DateTime.UtcNow,
                CalculatedBy = "SGKCalculationService",
                CalculationNotes = $"SGK hesaplama dönemi: {premiumDays} gün, Matrah: {sgkCalculationBasis:N2} TL",
                LegalReferences = "Sosyal Sigortalar ve Genel Sağlık Sigortası Kanunu (5510)"
            };

            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "SGK hesaplama hatası. Brüt: {Gross}", grossSalary);
            throw;
        }
    }

    public async Task<decimal> CalculateEmployeeContributionAsync(
        decimal grossSalary,
        int premiumDays = 30)
    {
        var basis = (grossSalary / 30m) * premiumDays;
        var contribution = basis * EMPLOYEE_SGK_RATE;
        return await Task.FromResult(contribution);
    }

    public async Task<decimal> CalculateEmployerContributionAsync(
        decimal grossSalary,
        int premiumDays = 30)
    {
        var basis = (grossSalary / 30m) * premiumDays;
        var contribution = basis * EMPLOYER_SGK_RATE;
        return await Task.FromResult(contribution);
    }

    public async Task<UnemploymentInsuranceDto> CalculateUnemploymentInsuranceAsync(
        decimal grossSalary,
        int premiumDays = 30)
    {
        var basis = (grossSalary / 30m) * premiumDays;

        return await Task.FromResult(new UnemploymentInsuranceDto
        {
            EmployeeRate = EMPLOYEE_UNEMPLOYMENT_RATE * 100,
            EmployeeAmount = basis * EMPLOYEE_UNEMPLOYMENT_RATE,
            EmployerRate = EMPLOYER_UNEMPLOYMENT_RATE * 100,
            EmployerAmount = basis * EMPLOYER_UNEMPLOYMENT_RATE,
            TotalAmount = (basis * EMPLOYEE_UNEMPLOYMENT_RATE) + (basis * EMPLOYER_UNEMPLOYMENT_RATE)
        });
    }

    public async Task<decimal> CalculateTotalEmployerCostAsync(
        decimal grossSalary,
        int premiumDays = 30)
    {
        var basis = (grossSalary / 30m) * premiumDays;
        var totalContribution = (basis * EMPLOYER_SGK_RATE) + (basis * EMPLOYER_UNEMPLOYMENT_RATE);
        var totalCost = grossSalary + totalContribution;
        return await Task.FromResult(totalCost);
    }

    public async Task<bool> IsExemptAsync(
        decimal grossSalary,
        string employmentType = "Regular")
    {
        // Türkiye'de belirli durumlar SGK'dan muaf olabilir
        // Genellikle muafiyet yoktur, bu method false döner
        return await Task.FromResult(false);
    }

    /// <summary>
    /// SGK muafiyeti uygulanmış sonuç
    /// </summary>
    private SGKCalculationResultDto CreateExemptResult(decimal grossSalary)
    {
        return new SGKCalculationResultDto
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
            EmployeeContributionBreakdown = "Muafiyet uygulandı",
            TotalEmployerContribution = 0,
            EmployerContributionBreakdown = "Muafiyet uygulandı",
            TotalEmployerCost = grossSalary,
            IsInsured = false,
            PremiumDays = 0,
            IsExempt = true,
            CalculationPeriod = $"{DateTime.UtcNow:MM/yyyy}",
            SGKTariffVersion = "2025-Turkey-v1",
            CalculatedDate = DateTime.UtcNow,
            CalculatedBy = "SGKCalculationService",
            CalculationNotes = "SGK muafiyeti uygulanmıştır.",
            LegalReferences = "Sosyal Sigortalar ve Genel Sağlık Sigortası Kanunu (5510)"
        };
    }
}