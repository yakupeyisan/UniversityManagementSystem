namespace UniversityMS.Application.Features.ProcurementFeature.DTOs;

public class PurchaseItemDto
{
    public Guid Id { get; set; }
    public string ItemName { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public string Unit { get; set; } = string.Empty;
    public decimal TotalPrice { get; set; }
}