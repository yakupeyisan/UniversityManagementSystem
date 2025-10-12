using UniversityMS.Domain.Exceptions;

namespace UniversityMS.Domain.ValueObjects;

/// <summary>
/// Çalışan Numarası Value Object
/// </summary>
public class EmployeeNumber : ValueObject
{
    public string Value { get; private set; } = null!;

    private EmployeeNumber() { }

    private EmployeeNumber(string value)
    {
        Value = value;
    }

    public static EmployeeNumber Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new DomainException("Çalışan numarası boş olamaz.");

        // Format: EMP-YYYYMMDD-XXXX (örn: EMP-20240101-0001)
        if (value.Length < 10)
            throw new DomainException("Çalışan numarası formatı geçersiz.");

        return new EmployeeNumber(value.ToUpperInvariant());
    }

    public static EmployeeNumber Generate(DateTime hireDate, int sequence)
    {
        var value = $"EMP-{hireDate:yyyyMMdd}-{sequence:D4}";
        return new EmployeeNumber(value);
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }

    public override string ToString() => Value;

    public static implicit operator string(EmployeeNumber employeeNumber) => employeeNumber.Value;
}