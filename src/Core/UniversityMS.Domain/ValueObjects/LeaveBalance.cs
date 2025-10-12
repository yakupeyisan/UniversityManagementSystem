using UniversityMS.Domain.Exceptions;

namespace UniversityMS.Domain.ValueObjects;

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