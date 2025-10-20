namespace UniversityMS.Application.Features.FinanceFeature.DTOs;

public class InvoiceItemDto
{
    public Guid Id { get; set; }
    public string Description { get; set; } = null!;
    public decimal Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal TaxRate { get; set; }
    public decimal TaxAmount { get; set; }
    public decimal TotalAmount { get; set; }
}