namespace UniversityMS.Application.Features.ProcurementFeature.DTOs;

/// <summary>
/// Tedarikçi Output DTO
/// </summary>
public class SupplierDto
{
    public Guid Id { get; set; }
    public string SupplierName { get; set; } = null!;
    public string SupplierCode { get; set; } = null!;
    public string ContactPerson { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string Phone { get; set; } = null!;
    public string Address { get; set; } = null!;
    public string City { get; set; } = null!;
    public string Country { get; set; } = null!;
    public string TaxNumber { get; set; } = null!;
    public string Status { get; set; } = null!;
    public decimal Rating { get; set; }
    public int TotalOrders { get; set; }
    public decimal TotalPurchaseAmount { get; set; }
}