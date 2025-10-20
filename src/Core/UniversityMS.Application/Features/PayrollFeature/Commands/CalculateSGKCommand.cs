using MediatR;
using UniversityMS.Application.Common.Models;
using UniversityMS.Application.Features.PayrollFeature.DTOs;

namespace UniversityMS.Application.Features.PayrollFeature.Commands;

/// <summary>
/// SGK Hesaplama Command
/// SGK çalışan ve işveren primlerini hesaplar (Türkiye)
/// </summary>
public record CalculateSGKCommand(
    /// <summary>Bordro ID'si</summary>
    Guid PayrollId,

    /// <summary>Brüt Maaş</summary>
    decimal GrossSalary,

    /// <summary>Prim Günü Sayısı (Ay içinde, default 30)</summary>
    int PremiumDays = 30,

    /// <summary>Sigortalı mı?</summary>
    bool IsInsured = true,

    /// <summary>Muafiyet var mı?</summary>
    bool IsExempt = false,

    /// <summary>Hesaplama Yılı</summary>
    int? CalculationYear = null,

    /// <summary>Hesaplama Ayı</summary>
    int? CalculationMonth = null
) : IRequest<Result<SGKCalculationResultDto>>;