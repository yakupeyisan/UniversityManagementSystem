using UniversityMS.Domain.Exceptions;

namespace UniversityMS.Domain.ValueObjects;

/// <summary>
/// İletişim Bilgisi Value Object
/// </summary>
public class ContactInfo : ValueObject
{
    public string? Phone { get; private set; }
    public string? Mobile { get; private set; }
    public string? Email { get; private set; }
    public string? Fax { get; private set; }
    public string? Website { get; private set; }

    private ContactInfo() { }

    private ContactInfo(string? phone, string? mobile, string? email, string? fax, string? website)
    {
        Phone = phone?.Trim();
        Mobile = mobile?.Trim();
        Email = email?.Trim();
        Fax = fax?.Trim();
        Website = website?.Trim();
    }

    public static ContactInfo Create(
        string? phone = null,
        string? mobile = null,
        string? email = null,
        string? fax = null,
        string? website = null)
    {
        // En az bir iletişim bilgisi olmalı
        if (string.IsNullOrWhiteSpace(phone) &&
            string.IsNullOrWhiteSpace(mobile) &&
            string.IsNullOrWhiteSpace(email))
        {
            throw new DomainException("En az bir iletişim bilgisi (telefon, mobil veya e-posta) girilmelidir.");
        }

        // Email formatı kontrolü
        if (!string.IsNullOrWhiteSpace(email) && !email.Contains("@"))
        {
            throw new DomainException("Geçersiz e-posta formatı.");
        }

        return new ContactInfo(phone, mobile, email, fax, website);
    }

    public ContactInfo UpdatePhone(string phone)
    {
        return Create(phone, Mobile, Email, Fax, Website);
    }

    public ContactInfo UpdateEmail(string email)
    {
        return Create(Phone, Mobile, email, Fax, Website);
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Phone ?? string.Empty;
        yield return Mobile ?? string.Empty;
        yield return Email ?? string.Empty;
    }

    public override string ToString()
    {
        var parts = new List<string>();
        if (!string.IsNullOrWhiteSpace(Phone)) parts.Add($"Tel: {Phone}");
        if (!string.IsNullOrWhiteSpace(Mobile)) parts.Add($"Mobil: {Mobile}");
        if (!string.IsNullOrWhiteSpace(Email)) parts.Add($"Email: {Email}");
        return string.Join(", ", parts);
    }
}