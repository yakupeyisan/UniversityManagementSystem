namespace UniversityMS.Application.Features.ProcurementFeature.DTOs;

/// <summary>
/// Satın Alma Siparişi Output DTO
/// </summary>
public class PurchaseOrderDto
{
    public Guid Id { get; set; }
    public string PurchaseOrderNumber { get; set; } = null!;
    public Guid SupplierId { get; set; }
    public string SupplierName { get; set; } = null!;
    public Guid PurchaseRequestId { get; set; }
    public string PurchaseRequestNumber { get; set; } = null!;
    public DateTime OrderDate { get; set; }
    public DateTime? DeliveryDate { get; set; }
    public decimal TotalAmount { get; set; }
    public decimal TaxAmount { get; set; }
    public decimal GrandTotal { get; set; }
    public string Status { get; set; } = null!;
    public string PaymentTerms { get; set; } = null!;
    public DateTime? ReceivedDate { get; set; }
    public List<PurchaseOrderItemDto> Items { get; set; } = new();
    public DateTime CreatedDate { get; set; }
}