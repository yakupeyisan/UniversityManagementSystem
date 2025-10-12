using UniversityMS.Domain.Entities.Common;
using UniversityMS.Domain.Exceptions;

namespace UniversityMS.Domain.Entities.PersonAggregate;

public class Address : BaseEntity
{
    public string Street { get; private set; }
    public string? BuildingNo { get; private set; }
    public string? ApartmentNo { get; private set; }
    public string District { get; private set; }
    public string City { get; private set; }
    public string PostalCode { get; private set; }
    public string Country { get; private set; }

    private Address() { } // EF Core için

    private Address(string street, string district, string city, string postalCode,
        string country, string? buildingNo = null, string? apartmentNo = null)
        : base()
    {
        Street = street;
        District = district;
        City = city;
        PostalCode = postalCode;
        Country = country;
        BuildingNo = buildingNo;
        ApartmentNo = apartmentNo;
    }

    public static Address Create(string street, string district, string city,
        string postalCode, string country = "Türkiye",
        string? buildingNo = null, string? apartmentNo = null)
    {
        if (string.IsNullOrWhiteSpace(street))
            throw new DomainException("Sokak bilgisi boş olamaz.");

        if (string.IsNullOrWhiteSpace(district))
            throw new DomainException("İlçe bilgisi boş olamaz.");

        if (string.IsNullOrWhiteSpace(city))
            throw new DomainException("İl bilgisi boş olamaz.");

        if (string.IsNullOrWhiteSpace(postalCode))
            throw new DomainException("Posta kodu boş olamaz.");

        return new Address(street.Trim(), district.Trim(), city.Trim(),
            postalCode.Trim(), country.Trim(), buildingNo?.Trim(), apartmentNo?.Trim());
    }

    public string GetFullAddress()
    {
        var parts = new List<string> { Street };

        if (!string.IsNullOrWhiteSpace(BuildingNo))
            parts.Add($"No: {BuildingNo}");

        if (!string.IsNullOrWhiteSpace(ApartmentNo))
            parts.Add($"Daire: {ApartmentNo}");

        parts.Add(District);
        parts.Add($"{City} {PostalCode}");
        parts.Add(Country);

        return string.Join(", ", parts);
    }
}