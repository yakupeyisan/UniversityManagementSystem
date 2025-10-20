namespace UniversityMS.Application.Features.ProcurementFeature.DTOs;

/// <summary>
/// Tedarikçi Output DTO
/// </summary>
public class SupplierDto
{
    public Guid Id { get; set; }
    public string Code { get; set; } = null!;
    public string Name { get; set; } = null!;
    public string? TaxNumber { get; set; }
    public string Type { get; set; } = null!;
    public string Status { get; set; } = null!;
    public string? Email { get; set; }
    public string? Phone { get; set; }
    public string? Address { get; set; }
    public int PaymentTermDays { get; set; }
    public decimal? CreditLimit { get; set; }
    public decimal Rating { get; set; }
    public DateTime CreatedDate { get; set; }
}