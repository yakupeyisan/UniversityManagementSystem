namespace UniversityMS.Application.Common.Models;

/// <summary>
/// Vergi dilimi DTO
/// </summary>
public class TaxBracketDto
{
    /// <summary>Minimum tuttar</summary>
    public decimal MinAmount { get; set; }

    /// <summary>Maksimum tutar (0 = sınırsız)</summary>
    public decimal MaxAmount { get; set; }

    /// <summary>Vergi oranı (%)</summary>
    public decimal TaxRate { get; set; }

    /// <summary>Kümülatif indirim</summary>
    public decimal CumulativeDeduction { get; set; }

    /// <summary>Açıklama</summary>
    public string Description { get; set; } = "";
}