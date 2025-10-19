using System.Text.RegularExpressions;
using UniversityMS.Domain.Exceptions;

namespace UniversityMS.Domain.ValueObjects;


public class EmployeeNumber : ValueObject
{
    private const string Pattern = @"^EMP\d{8}$"; // EMP + 8 rakam

    public string Value { get; }

    private EmployeeNumber(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new DomainException("Çalışan numarası boş olamaz.");

        var trimmedValue = value.Trim().ToUpper();

        if (!Regex.IsMatch(trimmedValue, Pattern))
            throw new DomainException(
                $"Çalışan numarası geçersiz format. " +
                $"Beklenen format: EMP00000000 (EMP + 8 rakam). " +
                $"Alınan değer: {value}");

        Value = trimmedValue;
    }

    /// <summary>
    /// Çalışan numarası oluştur
    /// </summary>
    public static EmployeeNumber Create(string value)
    {
        return new EmployeeNumber(value);
    }

    /// <summary>
    /// Otomatik numara üret (sequence number kullanarak)
    /// </summary>
    public static EmployeeNumber GenerateAutomatic(int sequenceNumber)
    {
        if (sequenceNumber <= 0 || sequenceNumber > 99999999)
            throw new DomainException(
                "Sequence number 1 ile 99999999 arasında olmalıdır.");

        var number = $"EMP{sequenceNumber:D8}";
        return new EmployeeNumber(number);
    }

    public override string ToString() => Value;

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }
}