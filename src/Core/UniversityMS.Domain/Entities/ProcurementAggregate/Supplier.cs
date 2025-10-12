using UniversityMS.Domain.Entities.Common;
using UniversityMS.Domain.Entities.PersonAggregate;
using UniversityMS.Domain.Enums;
using UniversityMS.Domain.Exceptions;
using UniversityMS.Domain.ValueObjects;

namespace UniversityMS.Domain.Entities.ProcurementAggregate;

/// <summary>
/// Tedarikçi (Supplier) Entity
/// </summary>
public class Supplier : AuditableEntity
{
    public string Code { get; private set; } = null!;
    public string Name { get; private set; } = null!;
    public string? TaxNumber { get; private set; }
    public string? TaxOffice { get; private set; }
    public SupplierType Type { get; private set; }
    public SupplierStatus Status { get; private set; }
    public ContactInfo ContactInfo { get; private set; } = null!;
    public Address Address { get; private set; } = null!;
    public string? BankAccount { get; private set; }
    public int PaymentTermDays { get; private set; }
    public decimal? CreditLimit { get; private set; }
    public decimal Rating { get; private set; }
    public string? Notes { get; private set; }

    private Supplier() { }

    private Supplier(
        string code,
        string name,
        SupplierType type,
        ContactInfo contactInfo,
        Address address,
        int paymentTermDays = 30,
        string? taxNumber = null,
        string? taxOffice = null)
    {
        Code = code;
        Name = name;
        Type = type;
        Status = SupplierStatus.Active;
        ContactInfo = contactInfo;
        Address = address;
        PaymentTermDays = paymentTermDays;
        TaxNumber = taxNumber;
        TaxOffice = taxOffice;
        Rating = 0;
    }

    public static Supplier Create(
        string code,
        string name,
        SupplierType type,
        ContactInfo contactInfo,
        Address address,
        int paymentTermDays = 30,
        string? taxNumber = null,
        string? taxOffice = null)
    {
        if (string.IsNullOrWhiteSpace(code))
            throw new DomainException("Tedarikçi kodu boş olamaz.");

        if (string.IsNullOrWhiteSpace(name))
            throw new DomainException("Tedarikçi adı boş olamaz.");

        if (paymentTermDays < 0)
            throw new DomainException("Ödeme vadesi negatif olamaz.");

        return new Supplier(code, name, type, contactInfo, address, paymentTermDays, taxNumber, taxOffice);
    }

    public void UpdateRating(decimal rating)
    {
        if (rating < 0 || rating > 5)
            throw new DomainException("Değerlendirme 0-5 arasında olmalıdır.");

        Rating = rating;
    }

    public void Activate() => Status = SupplierStatus.Active;
    public void Deactivate() => Status = SupplierStatus.Inactive;
    public void Blacklist() => Status = SupplierStatus.Blacklisted;
}