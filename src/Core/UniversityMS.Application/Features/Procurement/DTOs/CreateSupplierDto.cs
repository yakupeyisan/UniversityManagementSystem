namespace UniversityMS.Application.Features.Procurement.DTOs;

/// <summary>
/// Tedarikçi Oluşturma Input DTO
/// </summary>
public class CreateSupplierDto
{
    public string SupplierName { get; set; } = null!;
    public string SupplierCode { get; set; } = null!;
    public string ContactPerson { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string Phone { get; set; } = null!;
    public string Address { get; set; } = null!;
    public string City { get; set; } = null!;
    public string Country { get; set; } = null!;
    public string TaxNumber { get; set; } = null!;
}