namespace UniversityMS.Application.Features.Procurement.DTOs;

/// <summary>
/// Satın Alma Talebi Kalemi Oluşturma DTO
/// </summary>
public class CreatePurchaseRequestItemDto
{
    public string ItemDescription { get; set; } = null!;
    public decimal Quantity { get; set; }
    public string Unit { get; set; } = null!;
    public decimal EstimatedUnitPrice { get; set; }
    public string Specification { get; set; } = null!;
}