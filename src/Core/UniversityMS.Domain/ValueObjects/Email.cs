using UniversityMS.Domain.Enums;
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


/// <summary>
/// Personel numarası Value Object
/// Format: PER-2024-00001
/// </summary>
public class EmployeeNumber : ValueObject
{
    public string Value { get; private set; }

    private EmployeeNumber(string value)
    {
        Value = value;
    }

    public static EmployeeNumber Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new DomainException("Personel numarası boş olamaz.");

        if (!IsValid(value))
            throw new DomainException("Geçersiz personel numarası formatı. Format: PER-YYYY-XXXXX");

        return new EmployeeNumber(value);
    }

    public static EmployeeNumber Generate(int year, int sequence)
    {
        var value = $"PER-{year}-{sequence:D5}";
        return new EmployeeNumber(value);
    }

    private static bool IsValid(string value)
    {
        // Format: PER-2024-00001
        var pattern = @"^PER-\d{4}-\d{5}$";
        return System.Text.RegularExpressions.Regex.IsMatch(value, pattern);
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }

    public override string ToString() => Value;

    public static implicit operator string(EmployeeNumber employeeNumber) => employeeNumber.Value;
}

/// <summary>
/// Maaş bilgisi Value Object
/// </summary>
public class SalaryInfo : ValueObject
{
    public decimal BaseSalary { get; private set; }
    public decimal Allowances { get; private set; }
    public decimal Bonuses { get; private set; }
    public string Currency { get; private set; }

    private SalaryInfo(decimal baseSalary, decimal allowances, decimal bonuses, string currency)
    {
        BaseSalary = baseSalary;
        Allowances = allowances;
        Bonuses = bonuses;
        Currency = currency;
    }

    public static SalaryInfo Create(decimal baseSalary, decimal allowances = 0, decimal bonuses = 0, string currency = "TRY")
    {
        if (baseSalary < 0)
            throw new DomainException("Maaş negatif olamaz.");

        if (allowances < 0)
            throw new DomainException("Tazminatlar negatif olamaz.");

        if (bonuses < 0)
            throw new DomainException("Primler negatif olamaz.");

        if (string.IsNullOrWhiteSpace(currency))
            throw new DomainException("Para birimi belirtilmelidir.");

        return new SalaryInfo(baseSalary, allowances, bonuses, currency);
    }

    public decimal GetGrossSalary() => BaseSalary + Allowances + Bonuses;

    public decimal CalculateNetSalary(decimal taxRate, decimal sgkRate)
    {
        var grossSalary = GetGrossSalary();
        var totalDeductions = grossSalary * (taxRate + sgkRate);
        return grossSalary - totalDeductions;
    }

    public SalaryInfo UpdateBaseSalary(decimal newBaseSalary)
    {
        return Create(newBaseSalary, Allowances, Bonuses, Currency);
    }

    public SalaryInfo AddBonus(decimal bonus)
    {
        return Create(BaseSalary, Allowances, Bonuses + bonus, Currency);
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return BaseSalary;
        yield return Allowances;
        yield return Bonuses;
        yield return Currency;
    }
}

/// <summary>
/// Çalışma saatleri Value Object
/// </summary>
public class WorkingHours : ValueObject
{
    public TimeOnly StartTime { get; private set; }
    public TimeOnly EndTime { get; private set; }
    public int WeeklyHours { get; private set; }

    private WorkingHours(TimeOnly startTime, TimeOnly endTime, int weeklyHours)
    {
        StartTime = startTime;
        EndTime = endTime;
        WeeklyHours = weeklyHours;
    }

    public static WorkingHours Create(TimeOnly startTime, TimeOnly endTime, int weeklyHours = 40)
    {
        if (endTime <= startTime)
            throw new DomainException("Bitiş saati başlangıç saatinden büyük olmalıdır.");

        if (weeklyHours <= 0 || weeklyHours > 168)
            throw new DomainException("Haftalık çalışma saati 1-168 arasında olmalıdır.");

        return new WorkingHours(startTime, endTime, weeklyHours);
    }

    public static WorkingHours CreateStandard()
    {
        // Standart 08:00 - 17:00, 40 saat/hafta
        return new WorkingHours(new TimeOnly(8, 0), new TimeOnly(17, 0), 40);
    }

    public int GetDailyHours()
    {
        return (int)(EndTime - StartTime).TotalHours;
    }

    public bool IsWithinWorkingHours(TimeOnly time)
    {
        return time >= StartTime && time <= EndTime;
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return StartTime;
        yield return EndTime;
        yield return WeeklyHours;
    }
}

/// <summary>
/// İzin bakiyesi Value Object
/// </summary>
public class LeaveBalance : ValueObject
{
    public int TotalDays { get; private set; }
    public int UsedDays { get; private set; }
    public int RemainingDays => TotalDays - UsedDays;

    private LeaveBalance(int totalDays, int usedDays)
    {
        TotalDays = totalDays;
        UsedDays = usedDays;
    }

    public static LeaveBalance Create(int totalDays, int usedDays = 0)
    {
        if (totalDays < 0)
            throw new DomainException("Toplam izin günü negatif olamaz.");

        if (usedDays < 0)
            throw new DomainException("Kullanılan izin günü negatif olamaz.");

        if (usedDays > totalDays)
            throw new DomainException("Kullanılan izin, toplam izinden fazla olamaz.");

        return new LeaveBalance(totalDays, usedDays);
    }

    public LeaveBalance UseLeave(int days)
    {
        if (days <= 0)
            throw new DomainException("İzin günü pozitif olmalıdır.");

        if (UsedDays + days > TotalDays)
            throw new DomainException($"Yetersiz izin bakiyesi. Kalan: {RemainingDays} gün");

        return new LeaveBalance(TotalDays, UsedDays + days);
    }

    public LeaveBalance AddLeave(int days)
    {
        if (days <= 0)
            throw new DomainException("Eklenecek izin günü pozitif olmalıdır.");

        return new LeaveBalance(TotalDays + days, UsedDays);
    }

    public LeaveBalance ResetUsed()
    {
        return new LeaveBalance(TotalDays, 0);
    }

    public bool CanTakeLeave(int days)
    {
        return RemainingDays >= days;
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return TotalDays;
        yield return UsedDays;
    }
}

/// <summary>
/// Performans skoru Value Object
/// </summary>
public class PerformanceScore : ValueObject
{
    public decimal Score { get; private set; }
    public const decimal MinScore = 0;
    public const decimal MaxScore = 100;

    private PerformanceScore(decimal score)
    {
        Score = score;
    }

    public static PerformanceScore Create(decimal score)
    {
        if (score < MinScore || score > MaxScore)
            throw new DomainException($"Performans skoru {MinScore}-{MaxScore} arasında olmalıdır.");

        return new PerformanceScore(score);
    }

    public PerformanceRating GetRating()
    {
        return Score switch
        {
            < 40 => PerformanceRating.BelowExpectations,
            < 60 => PerformanceRating.NeedsImprovement,
            < 80 => PerformanceRating.MeetsExpectations,
            < 95 => PerformanceRating.ExceedsExpectations,
            _ => PerformanceRating.Outstanding
        };
    }

    public bool IsSatisfactory() => Score >= 60;

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Score;
    }

    public static implicit operator decimal(PerformanceScore score) => score.Score;
}

/// <summary>
/// Eğitim süresi Value Object
/// </summary>
public class TrainingDuration : ValueObject
{
    public int Hours { get; private set; }
    public int Days => (int)Math.Ceiling(Hours / 8.0);

    private TrainingDuration(int hours)
    {
        Hours = hours;
    }

    public static TrainingDuration Create(int hours)
    {
        if (hours <= 0)
            throw new DomainException("Eğitim süresi pozitif olmalıdır.");

        if (hours > 2000) // Max 1 yıl çalışma saati
            throw new DomainException("Eğitim süresi çok uzun.");

        return new TrainingDuration(hours);
    }

    public static TrainingDuration FromDays(int days)
    {
        return Create(days * 8);
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Hours;
    }

    public override string ToString() => $"{Hours} saat ({Days} gün)";
}