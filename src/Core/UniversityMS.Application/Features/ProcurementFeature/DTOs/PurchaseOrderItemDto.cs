namespace UniversityMS.Application.Features.ProcurementFeature.DTOs;

/// <summary>
/// Satın Alma Siparişi Kalemi DTO
/// </summary>
public class PurchaseOrderItemDto
{
    public Guid Id { get; set; }
    public string Description { get; set; } = null!;
    public decimal Quantity { get; set; }
    public string Unit { get; set; } = null!;
    public decimal UnitPrice { get; set; }
    public decimal TotalPrice { get; set; }
    public decimal ReceivedQuantity { get; set; }
    public string Status { get; set; } = null!;
}