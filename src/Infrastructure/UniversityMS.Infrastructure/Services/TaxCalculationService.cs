using Microsoft.Extensions.Logging;
using UniversityMS.Application.Common.Interfaces;
using UniversityMS.Application.Common.Models;
using UniversityMS.Application.Features.PayrollFeature.DTOs;

namespace UniversityMS.Infrastructure.Services;

/// <summary>
/// Vergi Hesaplama Servisi Implementation
/// Türkiye'deki gelir vergisini hesaplar
/// 2025 Türkiye Gelir Vergisi Tarifesi
/// </summary>
public class TaxCalculationService : ITaxCalculationService
{
    private readonly ILogger<TaxCalculationService> _logger;

    public TaxCalculationService(ILogger<TaxCalculationService> logger)
    {
        _logger = logger;
    }

    public async Task<TaxCalculationResultDto> CalculateTaxAsync(
        decimal grossSalary,
        decimal sgkDeductions,
        int? taxYear = null,
        int? taxMonth = null)
    {
        return await CalculateTaxDetailedAsync(
            grossSalary,
            sgkDeductions,
            taxDiscount: null,
            taxYear: taxYear,
            taxMonth: taxMonth);
    }

    public async Task<TaxCalculationResultDto> CalculateTaxDetailedAsync(
        decimal grossSalary,
        decimal sgkDeductions,
        decimal? taxDiscount = null,
        int? taxYear = null,
        int? taxMonth = null)
    {
        try
        {
            _logger.LogInformation(
                "Vergi hesaplama başlıyor. Brüt: {GrossSalary}, SGK: {SGK}",
                grossSalary, sgkDeductions);

            // ========== VALIDASYON ==========
            if (grossSalary <= 0)
                throw new ArgumentException("Brüt maaş sıfırdan büyük olmalıdır.");

            if (sgkDeductions < 0)
                throw new ArgumentException("SGK kesintileri negatif olamaz.");

            // ========== TARİH AYARLAMA ==========
            var calculatedYear = taxYear ?? DateTime.UtcNow.Year;
            var calculatedMonth = taxMonth ?? DateTime.UtcNow.Month;

            // ========== VERGİLENDİRİLECEK GELİR (Brüt - SGK) ==========
            decimal taxableIncome = await CalculateTaxableIncomeAsync(grossSalary, sgkDeductions);

            _logger.LogInformation(
                "Vergilendirilecek gelir: {TaxableIncome}", taxableIncome);

            // ========== UYGUN VERGİ ORANI ==========
            decimal taxRate = await GetApplicableTaxRateAsync(taxableIncome, calculatedYear);

            // ========== GELIR VERGİSİ HESAPLAMA ==========
            decimal incomeTax = CalculateProgressiveTax(taxableIncome, calculatedYear);

            _logger.LogInformation(
                "Hesaplanan vergi: {Tax}, Oran: {Rate}%",
                incomeTax, taxRate);

            // ========== VERGİ İNDİRİMİ ==========
            decimal discountAmount = 0m;
            if (taxDiscount.HasValue && taxDiscount.Value > 0)
            {
                discountAmount = incomeTax * (taxDiscount.Value / 100m);
                incomeTax = Math.Max(0, incomeTax - discountAmount);
            }

            // ========== DAMGA VERGİSİ (Bordro için %0.759) ==========
            const decimal stampDutyRate = 0.00759m; // %0.759
            decimal stampDuty = grossSalary * stampDutyRate;

            // ========== TOPLAM KESİNTİLER ==========
            decimal totalTaxDeductions = incomeTax + stampDuty;
            decimal netSalary = grossSalary - sgkDeductions - totalTaxDeductions;

            _logger.LogInformation(
                "Vergi hesaplama tamamlandı. Net: {NetSalary}",
                netSalary);

            // ========== SONUÇ ==========
            return new TaxCalculationResultDto
            {
                GrossSalary = grossSalary,
                SGKDeductions = sgkDeductions,
                TaxableIncome = taxableIncome,
                AdjustedTaxableIncome = taxableIncome,
                IncomeIncomeTaxRate = taxRate,
                IncomeTax = incomeTax,
                IncomeTaxDiscount = taxDiscount,
                AfterIncomeTax = taxableIncome - incomeTax,
                WithholdingTaxRate = 0, // Bordro için stopaj yok (normalde)
                WithholdingTax = 0,
                StampDutyRate = stampDutyRate * 100, // Yüzde olarak
                StampDuty = stampDuty,
                TemporaryTaxDeduction = 0,
                TotalTaxDeductions = totalTaxDeductions,
                NetAmountAfterTax = netSalary,
                TaxYear = calculatedYear,
                TaxMonth = calculatedMonth,
                TaxTableVersion = "2025-Turkey-v1",
                CalculatedDate = DateTime.UtcNow,
                CalculatedBy = "TaxCalculationService",
                CalculationNotes = $"Vergi dönem: {calculatedMonth:D2}/{calculatedYear}, Tarife: Artan Oranlı",
                ApplicableLaws = "Gelir Vergisi Kanunu - 2025 Türkiye"
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Vergi hesaplama hatası. Brüt: {Gross}", grossSalary);
            throw;
        }
    }

    public async Task<decimal> CalculateTaxableIncomeAsync(decimal grossSalary, decimal sgkDeductions)
    {
        // Vergilendirilecek gelir = Brüt Maaş - SGK Kesintileri
        var taxableIncome = grossSalary - sgkDeductions;
        return await Task.FromResult(Math.Max(0, taxableIncome));
    }

    public async Task<decimal> GetApplicableTaxRateAsync(decimal taxableIncome, int year)
    {
        var brackets = await GetTaxBracketsAsync(year);
        var applicableBracket = brackets.LastOrDefault(b => taxableIncome >= b.MinAmount);
        return await Task.FromResult(applicableBracket?.TaxRate ?? 0);
    }

    public async Task<IEnumerable<TaxBracketDto>> GetTaxBracketsAsync(int year)
    {
        // 2025 Türkiye Gelir Vergisi Tarifesi (Artan Oranlı)
        if (year >= 2025)
        {
            return await Task.FromResult(new List<TaxBracketDto>
            {
                new()
                {
                    MinAmount = 0,
                    MaxAmount = 37000,
                    TaxRate = 15m,
                    CumulativeDeduction = 0,
                    Description = "0 - 37.000 TL arası: %15"
                },
                new()
                {
                    MinAmount = 37000,
                    MaxAmount = 89000,
                    TaxRate = 20m,
                    CumulativeDeduction = 5550, // 37000 * 0.15
                    Description = "37.000 - 89.000 TL arası: %20"
                },
                new()
                {
                    MinAmount = 89000,
                    MaxAmount = 180000,
                    TaxRate = 27m,
                    CumulativeDeduction = 15750, // 5550 + (52000 * 0.20)
                    Description = "89.000 - 180.000 TL arası: %27"
                },
                new()
                {
                    MinAmount = 180000,
                    MaxAmount = decimal.MaxValue,
                    TaxRate = 35m,
                    CumulativeDeduction = 40320, // 15750 + (91000 * 0.27)
                    Description = "180.000 TL üzeri: %35"
                }
            });
        }

        // Fallback for other years
        return await Task.FromResult(new List<TaxBracketDto>
        {
            new()
            {
                MinAmount = 0,
                MaxAmount = decimal.MaxValue,
                TaxRate = 20m,
                CumulativeDeduction = 0,
                Description = "Varsayılan: %20"
            }
        });
    }

    /// <summary>
    /// Artan oranlı vergi hesaplama
    /// </summary>
    private decimal CalculateProgressiveTax(decimal taxableIncome, int year)
    {
        if (year >= 2025)
        {
            // 0 - 37.000: %15
            if (taxableIncome <= 37000)
                return taxableIncome * 0.15m;

            // 37.000 - 89.000: 5.550 + (üzeri * %20)
            if (taxableIncome <= 89000)
                return 5550 + ((taxableIncome - 37000) * 0.20m);

            // 89.000 - 180.000: 15.750 + (üzeri * %27)
            if (taxableIncome <= 180000)
                return 15750 + ((taxableIncome - 89000) * 0.27m);

            // 180.000+: 40.320 + (üzeri * %35)
            return 40320 + ((taxableIncome - 180000) * 0.35m);
        }

        // Fallback
        return taxableIncome * 0.20m;
    }
}