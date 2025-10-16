namespace UniversityMS.Application.Features.Procurement.DTOs;

/// <summary>
/// Satın Alma Talebi Kalemi DTO
/// </summary>
public class PurchaseRequestItemDto
{
    public Guid Id { get; set; }
    public string ItemDescription { get; set; } = null!;
    public decimal Quantity { get; set; }
    public string Unit { get; set; } = null!;
    public decimal EstimatedUnitPrice { get; set; }
    public decimal EstimatedTotalPrice { get; set; }
    public string Specification { get; set; } = null!;
    public int Priority { get; set; }
}