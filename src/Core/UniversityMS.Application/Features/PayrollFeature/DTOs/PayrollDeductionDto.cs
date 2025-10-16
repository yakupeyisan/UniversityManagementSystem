namespace UniversityMS.Application.Features.PayrollFeature.DTOs;

/// <summary>
/// Bordro Kesinti Output DTO
/// </summary>
public class PayrollDeductionDto
{
    public Guid Id { get; set; }
    public string Type { get; set; } = null!;
    public string Description { get; set; } = null!;
    public decimal Amount { get; set; }
    public decimal? Rate { get; set; }
    public bool IsStatutory { get; set; }
    public string? Reference { get; set; }
}