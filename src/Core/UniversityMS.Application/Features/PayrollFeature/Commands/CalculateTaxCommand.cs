using MediatR;
using UniversityMS.Application.Common.Models;
using UniversityMS.Application.Features.PayrollFeature.DTOs;

namespace UniversityMS.Application.Features.PayrollFeature.Commands;

/// <summary>
/// Gelir Vergisi Hesaplama Command
/// Brüt maaştan gelir vergisini hesaplar (Türkiye - 2025)
/// </summary>
public record CalculateTaxCommand(
    /// <summary>Bordro ID'si</summary>
    Guid PayrollId,

    /// <summary>Brüt Maaş</summary>
    decimal GrossSalary,

    /// <summary>SGK Kesintileri (varsa)</summary>
    decimal SGKDeductions = 0,

    /// <summary>Özel Vergi İndirimi (%)</summary>
    decimal? TaxDiscount = null,

    /// <summary>Hesaplama Yılı</summary>
    int? TaxYear = null,

    /// <summary>Hesaplama Ayı</summary>
    int? TaxMonth = null
) : IRequest<Result<TaxCalculationResultDto>>;