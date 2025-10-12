using UniversityMS.Domain.Enums;
using UniversityMS.Domain.Exceptions;

namespace UniversityMS.Domain.ValueObjects;

/// <summary>
/// Maaş Bilgisi Value Object
/// </summary>
public class SalaryInfo : ValueObject
{
    public decimal GrossSalary { get; private set; }
    public string Currency { get; private set; } = null!;
    public SalaryType Type { get; private set; }
    public decimal? HourlyRate { get; private set; }

    private SalaryInfo() { }

    private SalaryInfo(decimal grossSalary, string currency, SalaryType type, decimal? hourlyRate = null)
    {
        GrossSalary = grossSalary;
        Currency = currency;
        Type = type;
        HourlyRate = hourlyRate;
    }

    public static SalaryInfo Create(decimal grossSalary, string currency = "TRY", SalaryType type = SalaryType.Monthly)
    {
        if (grossSalary <= 0)
            throw new DomainException("Brüt maaş pozitif olmalıdır.");

        if (string.IsNullOrWhiteSpace(currency))
            throw new DomainException("Para birimi belirtilmelidir.");

        return new SalaryInfo(grossSalary, currency.ToUpperInvariant(), type);
    }

    public static SalaryInfo CreateHourly(decimal hourlyRate, string currency = "TRY")
    {
        if (hourlyRate <= 0)
            throw new DomainException("Saatlik ücret pozitif olmalıdır.");

        return new SalaryInfo(0, currency.ToUpperInvariant(), SalaryType.Hourly, hourlyRate);
    }

    public decimal GetGrossSalary()
    {
        if (Type == SalaryType.Hourly && HourlyRate.HasValue)
        {
            // Aylık 160 saat kabul edelim
            return HourlyRate.Value * 160;
        }
        return GrossSalary;
    }

    public decimal CalculateNetSalary(decimal taxRate = 15, decimal socialSecurityRate = 14)
    {
        var gross = GetGrossSalary();
        var totalDeductionRate = (taxRate + socialSecurityRate) / 100;
        return gross * (1 - totalDeductionRate);
    }

    public SalaryInfo IncreaseBySalary(decimal amount)
    {
        return Create(GrossSalary + amount, Currency, Type);
    }

    public SalaryInfo IncreaseByPercentage(decimal percentage)
    {
        var newSalary = GrossSalary * (1 + percentage / 100);
        return Create(newSalary, Currency, Type);
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return GrossSalary;
        yield return Currency;
        yield return Type;
    }

    public override string ToString() => $"{GrossSalary:N2} {Currency}";
}