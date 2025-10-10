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


/// <summary>
/// Çalışma Saatleri Value Object
/// </summary>
public class WorkingHours : ValueObject
{
    public TimeOnly StartTime { get; private set; }
    public TimeOnly EndTime { get; private set; }
    public int WeeklyHours { get; private set; }
    public int DailyHours { get; private set; }

    private WorkingHours() { }

    private WorkingHours(TimeOnly startTime, TimeOnly endTime, int weeklyHours)
    {
        StartTime = startTime;
        EndTime = endTime;
        WeeklyHours = weeklyHours;
        DailyHours = (int)(endTime - startTime).TotalHours;
    }

    public static WorkingHours Create(TimeOnly startTime, TimeOnly endTime, int weeklyHours = 40)
    {
        if (endTime <= startTime)
            throw new DomainException("Bitiş saati başlangıç saatinden sonra olmalıdır.");

        if (weeklyHours <= 0 || weeklyHours > 168)
            throw new DomainException("Haftalık çalışma saati 1-168 arasında olmalıdır.");

        return new WorkingHours(startTime, endTime, weeklyHours);
    }

    public static WorkingHours CreateStandard()
    {
        // Standart: 09:00-18:00, 40 saat/hafta
        return Create(new TimeOnly(9, 0), new TimeOnly(18, 0), 40);
    }

    public static WorkingHours CreateFlexible(int weeklyHours = 40)
    {
        // Esnek çalışma saatleri
        return Create(new TimeOnly(0, 0), new TimeOnly(23, 59), weeklyHours);
    }

    public bool IsFlexible()
    {
        return StartTime == new TimeOnly(0, 0) && EndTime == new TimeOnly(23, 59);
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return StartTime;
        yield return EndTime;
        yield return WeeklyHours;
    }

    public override string ToString() => IsFlexible()
        ? $"Esnek ({WeeklyHours} saat/hafta)"
        : $"{StartTime:HH:mm}-{EndTime:HH:mm} ({WeeklyHours} saat/hafta)";
}

/// <summary>
/// İzin Bakiyesi Value Object
/// </summary>
public class LeaveBalance : ValueObject
{
    public int TotalDays { get; private set; }
    public int UsedDays { get; private set; }
    public int RemainingDays { get; private set; }

    private LeaveBalance() { }

    private LeaveBalance(int totalDays, int usedDays = 0)
    {
        TotalDays = totalDays;
        UsedDays = usedDays;
        RemainingDays = totalDays - usedDays;
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
            throw new DomainException("Yetersiz izin bakiyesi.");

        return new LeaveBalance(TotalDays, UsedDays + days);
    }

    public LeaveBalance AddLeave(int days)
    {
        if (days <= 0)
            throw new DomainException("Eklenecek izin günü pozitif olmalıdır.");

        return new LeaveBalance(TotalDays + days, UsedDays);
    }

    public LeaveBalance RestoreLeave(int days)
    {
        if (days <= 0)
            throw new DomainException("İade edilecek izin günü pozitif olmalıdır.");

        if (days > UsedDays)
            throw new DomainException("İade edilecek gün, kullanılan günden fazla olamaz.");

        return new LeaveBalance(TotalDays, UsedDays - days);
    }

    public bool CanTakeLeave(int days)
    {
        return RemainingDays >= days;
    }

    public decimal GetUtilizationRate()
    {
        if (TotalDays == 0) return 0;
        return (decimal)UsedDays / TotalDays * 100;
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return TotalDays;
        yield return UsedDays;
    }

    public override string ToString() => $"{RemainingDays}/{TotalDays} gün kaldı";
}

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
/// <summary>
/// Performans Skoru Value Object
/// Skor ve Rating'i birlikte yönetir
/// </summary>
public class PerformanceScore : ValueObject
{
    public decimal Score { get; private set; }
    public PerformanceRating Rating { get; private set; }
    public string RatingDescription { get; private set; } = null!;
    public string ScoreRange { get; private set; } = null!;

    private PerformanceScore() { }

    private PerformanceScore(decimal score)
    {
        Score = Math.Round(score, 2);
        Rating = CalculateRating(score);
        RatingDescription = GetRatingDescription(Rating);
        ScoreRange = GetScoreRange(Rating);
    }

    /// <summary>
    /// Skor değerinden PerformanceScore oluşturur
    /// </summary>
    public static PerformanceScore Create(decimal score)
    {
        if (score < 0 || score > 100)
            throw new DomainException("Performans skoru 0-100 arasında olmalıdır.");

        return new PerformanceScore(score);
    }

    /// <summary>
    /// Rating'den PerformanceScore oluşturur (ortalama skor ile)
    /// </summary>
    public static PerformanceScore FromRating(PerformanceRating rating)
    {
        var averageScore = rating switch
        {
            PerformanceRating.Outstanding => 95m,
            PerformanceRating.ExceedsExpectations => 85m,
            PerformanceRating.MeetsExpectations => 75m,
            PerformanceRating.NeedsImprovement => 65m,
            PerformanceRating.Unsatisfactory => 50m,
            _ => throw new DomainException("Geçersiz performans derecesi.")
        };

        return new PerformanceScore(averageScore);
    }

    /// <summary>
    /// Birden fazla skorun ortalamasından oluşturur
    /// </summary>
    public static PerformanceScore FromAverage(params decimal[] scores)
    {
        if (scores == null || scores.Length == 0)
            throw new DomainException("En az bir skor belirtilmelidir.");

        var average = scores.Average();
        return Create(average);
    }

    /// <summary>
    /// Ağırlıklı ortalamadan oluşturur
    /// </summary>
    public static PerformanceScore FromWeightedAverage(Dictionary<decimal, decimal> scoreWeights)
    {
        if (scoreWeights == null || scoreWeights.Count == 0)
            throw new DomainException("En az bir skor ve ağırlık belirtilmelidir.");

        var totalWeight = scoreWeights.Values.Sum();
        if (Math.Abs(totalWeight - 100) > 0.01m)
            throw new DomainException("Ağırlıkların toplamı 100 olmalıdır.");

        var weightedSum = scoreWeights.Sum(kv => kv.Key * (kv.Value / 100));
        return Create(weightedSum);
    }

    private static PerformanceRating CalculateRating(decimal score)
    {
        return score switch
        {
            >= 90 => PerformanceRating.Outstanding,
            >= 80 => PerformanceRating.ExceedsExpectations,
            >= 70 => PerformanceRating.MeetsExpectations,
            >= 60 => PerformanceRating.NeedsImprovement,
            _ => PerformanceRating.Unsatisfactory
        };
    }

    private static string GetRatingDescription(PerformanceRating rating)
    {
        return rating switch
        {
            PerformanceRating.Outstanding => "Mükemmel",
            PerformanceRating.ExceedsExpectations => "Beklentilerin Üstünde",
            PerformanceRating.MeetsExpectations => "Beklentileri Karşılıyor",
            PerformanceRating.NeedsImprovement => "Geliştirilmeli",
            PerformanceRating.Unsatisfactory => "Yetersiz",
            _ => "Bilinmiyor"
        };
    }

    private static string GetScoreRange(PerformanceRating rating)
    {
        return rating switch
        {
            PerformanceRating.Outstanding => "90-100",
            PerformanceRating.ExceedsExpectations => "80-89",
            PerformanceRating.MeetsExpectations => "70-79",
            PerformanceRating.NeedsImprovement => "60-69",
            PerformanceRating.Unsatisfactory => "0-59",
            _ => "N/A"
        };
    }

    #region Helper Methods

    /// <summary>
    /// Mükemmel performans mı?
    /// </summary>
    public bool IsExcellent() => Rating == PerformanceRating.Outstanding;

    /// <summary>
    /// İyi performans mı? (Beklentileri karşılıyor veya üstünde)
    /// </summary>
    public bool IsGood() => Rating >= PerformanceRating.MeetsExpectations;

    /// <summary>
    /// Geliştirilmesi gerekiyor mu?
    /// </summary>
    public bool NeedsImprovement() => Rating <= PerformanceRating.NeedsImprovement;

    /// <summary>
    /// Yetersiz mi?
    /// </summary>
    public bool IsUnsatisfactory() => Rating == PerformanceRating.Unsatisfactory;

    /// <summary>
    /// Geçer not mu? (70 ve üzeri)
    /// </summary>
    public bool IsPassing() => Score >= 70;

    /// <summary>
    /// Başka bir skorla karşılaştır
    /// </summary>
    public bool IsBetterThan(PerformanceScore other) => Score > other.Score;

    /// <summary>
    /// Yüzdelik dilim hesapla (0-100 arası norm için)
    /// </summary>
    public int GetPercentile() => (int)Score;

    /// <summary>
    /// Rating'in numerik değerini al (1-5)
    /// </summary>
    public int GetNumericRating() => (int)Rating;

    #endregion

    #region Arithmetic Operations

    /// <summary>
    /// İki skorun ortalamasını al
    /// </summary>
    public static PerformanceScore Average(PerformanceScore score1, PerformanceScore score2)
    {
        var avg = (score1.Score + score2.Score) / 2;
        return Create(avg);
    }

    /// <summary>
    /// Skoru güncelle (yeni değerlendirme)
    /// </summary>
    public PerformanceScore UpdateScore(decimal newScore)
    {
        return Create(newScore);
    }

    /// <summary>
    /// İki skorun ağırlıklı ortalaması
    /// </summary>
    public static PerformanceScore WeightedAverage(
        PerformanceScore score1, decimal weight1,
        PerformanceScore score2, decimal weight2)
    {
        if (weight1 + weight2 != 100)
            throw new DomainException("Ağırlıkların toplamı 100 olmalıdır.");

        var weighted = (score1.Score * weight1 + score2.Score * weight2) / 100;
        return Create(weighted);
    }

    #endregion

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Score;
        yield return Rating;
    }

    public override string ToString() => $"{Score:F2} - {RatingDescription} ({ScoreRange})";
}

/// <summary>
/// Eğitim Süresi Value Object
/// </summary>
public class TrainingDuration : ValueObject
{
    public int Hours { get; private set; }
    public int Days { get; private set; }

    private TrainingDuration() { }

    private TrainingDuration(int hours, int days)
    {
        Hours = hours;
        Days = days;
    }

    public static TrainingDuration Create(int hours, int days = 0)
    {
        if (hours < 0)
            throw new DomainException("Eğitim saati negatif olamaz.");

        if (days < 0)
            throw new DomainException("Eğitim günü negatif olamaz.");

        return new TrainingDuration(hours, days);
    }

    public static TrainingDuration FromHours(int totalHours)
    {
        if (totalHours < 0)
            throw new DomainException("Toplam saat negatif olamaz.");

        var days = totalHours / 8;  // 8 saatlik iş günü
        var hours = totalHours % 8;

        return new TrainingDuration(hours, days);
    }

    public int GetTotalHours()
    {
        return (Days * 8) + Hours;
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Hours;
        yield return Days;
    }

    public override string ToString()
    {
        if (Days > 0)
            return $"{Days} gün {Hours} saat";
        return $"{Hours} saat";
    }
}

/// <summary>
/// Bordro Dönemi Value Object
/// </summary>
public class PayrollPeriod : ValueObject
{
    public int Year { get; private set; }
    public int Month { get; private set; }
    public string PeriodName { get; private set; } = null!;
    public DateTime StartDate { get; private set; }
    public DateTime EndDate { get; private set; }

    private PayrollPeriod() { }

    private PayrollPeriod(int year, int month)
    {
        Year = year;
        Month = month;
        PeriodName = $"{year}-{month:D2}";
        StartDate = new DateTime(year, month, 1);
        EndDate = StartDate.AddMonths(1).AddDays(-1);
    }

    public static PayrollPeriod Create(int year, int month)
    {
        if (year < 2000 || year > 2100)
            throw new DomainException("Geçerli bir yıl giriniz.");

        if (month < 1 || month > 12)
            throw new DomainException("Ay 1-12 arasında olmalıdır.");

        return new PayrollPeriod(year, month);
    }

    public static PayrollPeriod Current()
    {
        var now = DateTime.UtcNow;
        return Create(now.Year, now.Month);
    }

    public PayrollPeriod Previous()
    {
        var previousMonth = StartDate.AddMonths(-1);
        return Create(previousMonth.Year, previousMonth.Month);
    }

    public PayrollPeriod Next()
    {
        var nextMonth = StartDate.AddMonths(1);
        return Create(nextMonth.Year, nextMonth.Month);
    }

    public bool IsCurrent()
    {
        var now = DateTime.UtcNow;
        return Year == now.Year && Month == now.Month;
    }

    public bool IsPast()
    {
        var now = DateTime.UtcNow;
        return Year < now.Year || (Year == now.Year && Month < now.Month);
    }

    public int GetWorkingDays()
    {
        int workingDays = 0;
        for (var date = StartDate; date <= EndDate; date = date.AddDays(1))
        {
            if (date.DayOfWeek != DayOfWeek.Saturday && date.DayOfWeek != DayOfWeek.Sunday)
                workingDays++;
        }
        return workingDays;
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Year;
        yield return Month;
    }

    public override string ToString() => PeriodName;
}

