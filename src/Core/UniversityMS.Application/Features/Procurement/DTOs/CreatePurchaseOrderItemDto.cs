namespace UniversityMS.Application.Features.Procurement.DTOs;

/// <summary>
/// Satın Alma Siparişi Kalemi Oluşturma DTO
/// </summary>
public class CreatePurchaseOrderItemDto
{
    public Guid PurchaseRequestItemId { get; set; }
    public decimal Quantity { get; set; }
    public decimal UnitPrice { get; set; }
}