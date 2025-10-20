using UniversityMS.Application.Features.PayrollFeature.DTOs;

namespace UniversityMS.Application.Common.Interfaces;

/// <summary>
/// SGK (Sosyal Güvenlik Kurumu) Hesaplama Servisi Interface
/// Türkiye'de çalışan ve işveren SGK primlerini hesaplar
/// </summary>
public interface ISGKCalculationService
{
    /// <summary>
    /// SGK primlerini hesapla (Çalışan + İşveren + İşsizlik Sigortası)
    /// </summary>
    /// <param name="grossSalary">Brüt maaş</param>
    /// <param name="premiumDays">Prim ödenen gün sayısı (1-31)</param>
    /// <param name="isInsured">Sigortalı mı?</param>
    /// <returns>SGK hesaplama sonucu</returns>
    Task<SGKCalculationResultDto> CalculateSGKAsync(
        decimal grossSalary,
        int premiumDays = 30,
        bool isInsured = true);

    /// <summary>
    /// Çalışan SGK primini hesapla (%14)
    /// </summary>
    Task<decimal> CalculateEmployeeContributionAsync(
        decimal grossSalary,
        int premiumDays = 30);

    /// <summary>
    /// İşveren SGK primini hesapla (%22.2)
    /// </summary>
    Task<decimal> CalculateEmployerContributionAsync(
        decimal grossSalary,
        int premiumDays = 30);

    /// <summary>
    /// İşsizlik sigortası primini hesapla (Çalışan: %1, İşveren: %2)
    /// </summary>
    Task<UnemploymentInsuranceDto> CalculateUnemploymentInsuranceAsync(
        decimal grossSalary,
        int premiumDays = 30);

    /// <summary>
    /// Toplam işveren maliyetini hesapla (Brüt + İşveren SGK + İşsizlik)
    /// </summary>
    Task<decimal> CalculateTotalEmployerCostAsync(
        decimal grossSalary,
        int premiumDays = 30);

    /// <summary>
    /// SGK muafiyetini kontrol et
    /// </summary>
    Task<bool> IsExemptAsync(
        decimal grossSalary,
        string employmentType = "Regular");
}