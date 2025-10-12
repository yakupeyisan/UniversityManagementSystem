using UniversityMS.Domain.Exceptions;

namespace UniversityMS.Domain.ValueObjects;

public sealed class PhoneNumber : ValueObject
{
    public string Value { get; }

    private PhoneNumber(string value)
    {
        Value = value;
    }

    public static PhoneNumber Create(string phoneNumber)
    {
        if (string.IsNullOrWhiteSpace(phoneNumber))
            throw new DomainException("Telefon numarası boş olamaz.");

        // Sadece rakamları al
        var cleaned = new string(phoneNumber.Where(char.IsDigit).ToArray());

        if (cleaned.Length < 10 || cleaned.Length > 11)
            throw new DomainException("Telefon numarası 10 veya 11 haneli olmalıdır.");

        // Türkiye için 0 ile başlamalı veya 90 ile başlamalı
        if (!cleaned.StartsWith("0") && !cleaned.StartsWith("90"))
            throw new DomainException("Geçersiz telefon numarası formatı.");

        return new PhoneNumber(cleaned);
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }

    public string GetFormatted()
    {
        // Format: 0(XXX) XXX XX XX
        if (Value.Length == 11 && Value.StartsWith("0"))
        {
            return $"0({Value.Substring(1, 3)}) {Value.Substring(4, 3)} {Value.Substring(7, 2)} {Value.Substring(9, 2)}";
        }
        return Value;
    }

    public override string ToString() => Value;

    public static implicit operator string(PhoneNumber phoneNumber) => phoneNumber.Value;
}