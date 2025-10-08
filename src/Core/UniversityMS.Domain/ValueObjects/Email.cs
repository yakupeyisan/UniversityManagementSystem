using UniversityMS.Domain.Exceptions;

namespace UniversityMS.Domain.ValueObjects;

public sealed class Email : ValueObject
{
    public string Value { get; }

    private Email(string value)
    {
        Value = value;
    }

    public static Email Create(string email)
    {
        if (string.IsNullOrWhiteSpace(email))
            throw new DomainException("Email adresi boş olamaz.");

        email = email.Trim().ToLowerInvariant();

        if (!IsValidEmail(email))
            throw new DomainException("Geçersiz email formatı.");

        return new Email(email);
    }

    private static bool IsValidEmail(string email)
    {
        try
        {
            var addr = new System.Net.Mail.MailAddress(email);
            return addr.Address == email;
        }
        catch
        {
            return false;
        }
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }

    public override string ToString() => Value;

    public static implicit operator string(Email email) => email.Value;
}

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
        if (amount < 0)
            throw new DomainException("Tutar negatif olamaz.");

        if (string.IsNullOrWhiteSpace(currency))
            throw new DomainException("Para birimi belirtilmelidir.");

        return new Money(Math.Round(amount, 2), currency.ToUpperInvariant());
    }

    public static Money Zero(string currency = "TRY") => new Money(0, currency);

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

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Amount;
        yield return Currency;
    }

    public override string ToString() => $"{Amount:N2} {Currency}";
}

public sealed class DateRange : ValueObject
{
    public DateTime StartDate { get; }
    public DateTime EndDate { get; }

    private DateRange(DateTime startDate, DateTime endDate)
    {
        StartDate = startDate;
        EndDate = endDate;
    }

    public static DateRange Create(DateTime startDate, DateTime endDate)
    {
        if (startDate > endDate)
            throw new DomainException("Başlangıç tarihi bitiş tarihinden sonra olamaz.");

        return new DateRange(startDate.Date, endDate.Date);
    }

    public int GetDurationInDays()
    {
        return (EndDate - StartDate).Days + 1;
    }

    public bool Contains(DateTime date)
    {
        var checkDate = date.Date;
        return checkDate >= StartDate && checkDate <= EndDate;
    }

    public bool Overlaps(DateRange other)
    {
        return StartDate <= other.EndDate && EndDate >= other.StartDate;
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return StartDate;
        yield return EndDate;
    }

    public override string ToString() => $"{StartDate:dd.MM.yyyy} - {EndDate:dd.MM.yyyy}";
}

public sealed class GradeScore : ValueObject
{
    public double NumericScore { get; }
    public string LetterGrade { get; }
    public double GradePoint { get; }

    private GradeScore(double numericScore, string letterGrade, double gradePoint)
    {
        NumericScore = numericScore;
        LetterGrade = letterGrade;
        GradePoint = gradePoint;
    }

    public static GradeScore Create(double numericScore)
    {
        if (numericScore < 0 || numericScore > 100)
            throw new DomainException("Not 0-100 arasında olmalıdır.");

        var (letterGrade, gradePoint) = CalculateLetterGrade(numericScore);
        return new GradeScore(numericScore, letterGrade, gradePoint);
    }

    private static (string Letter, double Point) CalculateLetterGrade(double score)
    {
        return score switch
        {
            >= 90 => ("AA", 4.0),
            >= 85 => ("BA", 3.5),
            >= 80 => ("BB", 3.0),
            >= 75 => ("CB", 2.5),
            >= 70 => ("CC", 2.0),
            >= 65 => ("DC", 1.5),
            >= 60 => ("DD", 1.0),
            >= 50 => ("FD", 0.5),
            _ => ("FF", 0.0)
        };
    }

    public bool IsPassing() => GradePoint >= 2.0;

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return NumericScore;
        yield return LetterGrade;
        yield return GradePoint;
    }

    public override string ToString() => $"{NumericScore:F2} ({LetterGrade})";
}

public abstract class ValueObject
{
    protected abstract IEnumerable<object> GetEqualityComponents();

    public override bool Equals(object? obj)
    {
        if (obj == null || obj.GetType() != GetType())
            return false;

        var other = (ValueObject)obj;

        return GetEqualityComponents().SequenceEqual(other.GetEqualityComponents());
    }

    public override int GetHashCode()
    {
        return GetEqualityComponents()
            .Select(x => x?.GetHashCode() ?? 0)
            .Aggregate((x, y) => x ^ y);
    }

    public static bool operator ==(ValueObject? left, ValueObject? right)
    {
        if (left is null && right is null)
            return true;

        if (left is null || right is null)
            return false;

        return left.Equals(right);
    }

    public static bool operator !=(ValueObject? left, ValueObject? right)
    {
        return !(left == right);
    }
}