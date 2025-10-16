namespace UniversityMS.Application.Features.PayrollFeature.DTOs;

/// <summary>
/// Bordro Kesinti Input DTO
/// </summary>
public class PayrollDeductionInputDto
{
    public string Type { get; set; } = null!; // SGK, Vergi, vs.
    public decimal Amount { get; set; }
    public decimal? Rate { get; set; }
    public string? Description { get; set; }
}