using UniversityMS.Domain.Exceptions;

namespace UniversityMS.Domain.ValueObjects;

public sealed class Money : ValueObject
{
    public decimal Amount { get; }
    public string Currency { get; }

    private Money(decimal amount, string currency)
    {
        Amount = amount;
        Currency = currency;
    }

    public static Money Create(decimal amount, string currency = "TRY")
    {
        if (string.IsNullOrWhiteSpace(currency))
            throw new DomainException("Para birimi boş olamaz.");

        if (currency.Length != 3)
            throw new DomainException("Para birimi 3 karakterli ISO kodu olmalıdır.");

        return new Money(amount, currency.ToUpperInvariant());
    }

    public static Money Zero(string currency = "TRY")
    {
        return new Money(0, currency);
    }

    public Money Add(Money other)
    {
        if (Currency != other.Currency)
            throw new DomainException($"Farklı para birimleri toplanamaz: {Currency} != {other.Currency}");

        return new Money(Amount + other.Amount, Currency);
    }

    public Money Subtract(Money other)
    {
        if (Currency != other.Currency)
            throw new DomainException($"Farklı para birimleri çıkarılamaz: {Currency} != {other.Currency}");

        var result = Amount - other.Amount;
        if (result < 0)
            throw new DomainException("Sonuç negatif olamaz.");

        return new Money(result, Currency);
    }

    public Money Multiply(decimal multiplier)
    {
        if (multiplier < 0)
            throw new DomainException("Çarpan negatif olamaz.");

        return new Money(Amount * multiplier, Currency);
    }

    #region Arithmetic Operations with Currency Check

    public static Money operator +(Money a, Money b)
    {
        ValidateSameCurrency(a, b);
        return new Money(a.Amount + b.Amount, a.Currency);
    }

    public static Money operator -(Money a, Money b)
    {
        ValidateSameCurrency(a, b);
        return new Money(a.Amount - b.Amount, a.Currency);
    }

    public static Money operator *(Money money, decimal multiplier)
    {
        return new Money(money.Amount * multiplier, money.Currency);
    }

    public static Money operator /(Money money, decimal divisor)
    {
        if (divisor == 0)
            throw new DomainException("Sıfıra bölme hatası.");

        return new Money(money.Amount / divisor, money.Currency);
    }

    #endregion

    #region Comparison Operators with Currency Check

    public static bool operator >(Money a, Money b)
    {
        ValidateSameCurrency(a, b);
        return a.Amount > b.Amount;
    }

    public static bool operator <(Money a, Money b)
    {
        ValidateSameCurrency(a, b);
        return a.Amount < b.Amount;
    }

    public static bool operator >=(Money a, Money b)
    {
        ValidateSameCurrency(a, b);
        return a.Amount >= b.Amount;
    }

    public static bool operator <=(Money a, Money b)
    {
        ValidateSameCurrency(a, b);
        return a.Amount <= b.Amount;
    }

    public static bool operator ==(Money? a, Money? b)
    {
        if (a is null && b is null) return true;
        if (a is null || b is null) return false;
        return a.Amount == b.Amount && a.Currency == b.Currency;
    }

    public static bool operator !=(Money? a, Money? b)
    {
        return !(a == b);
    }

    #endregion

    #region Helper Methods

    private static void ValidateSameCurrency(Money a, Money b)
    {
        if (a.Currency != b.Currency)
            throw new DomainException($"Farklı para birimleri ({a.Currency} ve {b.Currency}) ile işlem yapılamaz.");
    }

    public bool IsZero() => Amount == 0;

    public bool IsPositive() => Amount > 0;

    public bool IsNegative() => Amount < 0;

    public Money Abs()
    {
        return new Money(Math.Abs(Amount), Currency);
    }

    public Money Negate()
    {
        return new Money(-Amount, Currency);
    }

    public Money Round(int decimals = 2)
    {
        return new Money(Math.Round(Amount, decimals), Currency);
    }

    /// <summary>
    /// Para birimini değiştir (döviz çevirisi DEĞİL, sadece para birimi değişikliği)
    /// </summary>
    public Money ChangeCurrency(string newCurrency)
    {
        return new Money(Amount, newCurrency);
    }

    /// <summary>
    /// Döviz çevirimi yap
    /// </summary>
    public Money ConvertTo(string targetCurrency, decimal exchangeRate)
    {
        if (exchangeRate <= 0)
            throw new DomainException("Döviz kuru pozitif olmalıdır.");

        return new Money(Amount * exchangeRate, targetCurrency);
    }

    #endregion

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Amount;
        yield return Currency;
    }

    public override string ToString() => $"{Amount:N2} {Currency}";

    public override bool Equals(object? obj)
    {
        return base.Equals(obj);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Amount, Currency);
    }
}