using UniversityMS.Application.Common.Models;
using UniversityMS.Application.Features.PayrollFeature.DTOs;

namespace UniversityMS.Application.Common.Interfaces;

/// <summary>
/// Vergi Hesaplama Servisi Interface
/// Türkiye'deki gelir vergisi, damga vergisi ve diğer vergileri hesaplar
/// </summary>
public interface ITaxCalculationService
{
    /// <summary>
    /// Brüt maaştan vergiyi hesapla
    /// </summary>
    /// <param name="grossSalary">Brüt maaş</param>
    /// <param name="sgkDeductions">SGK kesintileri</param>
    /// <param name="taxYear">Vergi yılı (default: cari yıl)</param>
    /// <param name="taxMonth">Vergi ayı (default: cari ay)</param>
    /// <returns>Vergi hesaplama sonucu</returns>
    Task<TaxCalculationResultDto> CalculateTaxAsync(
        decimal grossSalary,
        decimal sgkDeductions,
        int? taxYear = null,
        int? taxMonth = null);

    /// <summary>
    /// Detaylı vergi hesaplama (tüm parametreler ile)
    /// </summary>
    Task<TaxCalculationResultDto> CalculateTaxDetailedAsync(
        decimal grossSalary,
        decimal sgkDeductions,
        decimal? taxDiscount = null,
        int? taxYear = null,
        int? taxMonth = null);

    /// <summary>
    /// Vergi matrahını hesapla (SGK kesintisi yapıldıktan sonra)
    /// </summary>
    Task<decimal> CalculateTaxableIncomeAsync(decimal grossSalary, decimal sgkDeductions);

    /// <summary>
    /// Uygun vergi oranını döndür
    /// </summary>
    Task<decimal> GetApplicableTaxRateAsync(decimal taxableIncome, int year);

    /// <summary>
    /// Türkiye vergi tarifesini getir (yıla göre)
    /// </summary>
    Task<IEnumerable<TaxBracketDto>> GetTaxBracketsAsync(int year);
}