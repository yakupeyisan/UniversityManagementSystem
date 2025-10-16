namespace UniversityMS.Application.Features.PayrollFeature.DTOs;

/// <summary>
/// Bordro Kalemi Output DTO
/// </summary>
public class PayrollItemDto
{
    public Guid Id { get; set; }
    public string Type { get; set; } = null!;
    public string Category { get; set; } = null!;
    public string Description { get; set; } = null!;
    public decimal Amount { get; set; }
    public decimal? Quantity { get; set; }
    public bool IsTaxable { get; set; }
}