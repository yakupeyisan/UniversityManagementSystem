namespace UniversityMS.Application.Features.ProcurementFeature.DTOs;

/// <summary>
/// Satın Alma Siparişi Oluşturma Input DTO
/// </summary>
public class CreatePurchaseOrderDto
{
    public Guid PurchaseRequestId { get; set; }
    public Guid SupplierId { get; set; }
    public DateTime DeliveryDate { get; set; }
    public string PaymentTerms { get; set; } = null!;
    public List<CreatePurchaseOrderItemDto> Items { get; set; } = new();
    public string? Notes { get; set; }
}