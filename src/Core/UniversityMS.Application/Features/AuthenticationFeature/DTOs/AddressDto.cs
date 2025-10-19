namespace UniversityMS.Application.Features.AuthenticationFeature.DTOs;

public class AddressDto
{
    public string Street { get; set; } = string.Empty;
    public string? BuildingNo { get; set; }
    public string? ApartmentNo { get; set; }
    public string District { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public string PostalCode { get; set; } = string.Empty;
    public string Country { get; set; } = string.Empty;
}