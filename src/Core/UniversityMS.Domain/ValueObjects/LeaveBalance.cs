using UniversityMS.Domain.Enums;
using UniversityMS.Domain.Exceptions;

namespace UniversityMS.Domain.ValueObjects;

/// <summary>
/// İzin Bakiyesi Value Object
/// Tüm leave type'larını ve bakiyelerini yönetir
/// </summary>
public class LeaveBalance : ValueObject
{
    // Tahsis edilen günler
    public int AnnualLeaveDays { get; private set; }      // Yıllık izin
    public int SickLeaveDays { get; private set; }        // Raporlu/Hasta izni
    public int CompensatoryLeaveDays { get; private set; } // Telafi izni

    // Kullanılan günler (TİP BAZINDA)
    public int AnnualLeaveUsedDays { get; private set; }
    public int SickLeaveUsedDays { get; private set; }
    public int CompensatoryLeaveUsedDays { get; private set; }

    // Toplam kullanılan (hesaplanmış)
    public int UsedDays => AnnualLeaveUsedDays + SickLeaveUsedDays + CompensatoryLeaveUsedDays;

    // Calculated properties
    public int AnnualLeaveRemainingDays => AnnualLeaveDays - AnnualLeaveUsedDays;
    public int SickLeaveRemainingDays => SickLeaveDays - SickLeaveUsedDays;
    public int CompensatoryLeaveRemainingDays => CompensatoryLeaveDays - CompensatoryLeaveUsedDays;

    // ✅ FIX: Constructor'u düzelt - tüm parametreleri al
    private LeaveBalance(
        int annualLeaveDays,
        int sickLeaveDays,
        int compensatoryLeaveDays,
        int annualLeaveUsedDays = 0,
        int sickLeaveUsedDays = 0,
        int compensatoryLeaveUsedDays = 0)
    {
        AnnualLeaveDays = annualLeaveDays;
        SickLeaveDays = sickLeaveDays;
        CompensatoryLeaveDays = compensatoryLeaveDays;
        AnnualLeaveUsedDays = annualLeaveUsedDays;
        SickLeaveUsedDays = sickLeaveUsedDays;
        CompensatoryLeaveUsedDays = compensatoryLeaveUsedDays;
    }

    /// <summary>
    /// Yeni çalışan için ilk izin bakiyesi (14 gün yıllık, 7 gün raporlu)
    /// </summary>
    public static LeaveBalance CreateForNewEmployee()
    {
        return new LeaveBalance(
            annualLeaveDays: 14,           // Türkiye mevzuatı: 14 gün minimum
            sickLeaveDays: 7,              // Raporlu izin
            compensatoryLeaveDays: 0,      // Başlangıçta 0
            annualLeaveUsedDays: 0,
            sickLeaveUsedDays: 0,
            compensatoryLeaveUsedDays: 0);
    }

    /// <summary>
    /// Custom bakiye oluştur
    /// </summary>
    public static LeaveBalance Create(
        int annualLeaveDays,
        int sickLeaveDays = 7,
        int compensatoryLeaveDays = 0,
        int annualLeaveUsedDays = 0,
        int sickLeaveUsedDays = 0,
        int compensatoryLeaveUsedDays = 0)
    {
        if (annualLeaveDays < 0 || sickLeaveDays < 0 || compensatoryLeaveDays < 0)
            throw new DomainException("İzin günü negatif olamaz.");

        if (annualLeaveUsedDays < 0 || sickLeaveUsedDays < 0 || compensatoryLeaveUsedDays < 0)
            throw new DomainException("Kullanılan izin günü negatif olamaz.");

        return new LeaveBalance(
            annualLeaveDays,
            sickLeaveDays,
            compensatoryLeaveDays,
            annualLeaveUsedDays,
            sickLeaveUsedDays,
            compensatoryLeaveUsedDays);
    }

    /// <summary>
    /// Kalan gün sayısını döndür (hepsini topla)
    /// </summary>
    public int GetRemainingDays()
    {
        return AnnualLeaveRemainingDays + SickLeaveRemainingDays + CompensatoryLeaveRemainingDays;
    }

    /// <summary>
    /// Belirtilen leave type'ı için kalan gün sayısını döndür
    /// </summary>
    public int GetRemainingDays(LeaveType leaveType)
    {
        return leaveType switch
        {
            LeaveType.Annual => AnnualLeaveRemainingDays,
            LeaveType.Sick => SickLeaveRemainingDays,
            LeaveType.Compensatory => CompensatoryLeaveRemainingDays,
            _ => 0
        };
    }

    /// <summary>
    /// Belirtilen miktar izin alınabilir mi kontrol et (toplam)
    /// </summary>
    public bool CanTakeLeave(int daysRequested)
    {
        if (daysRequested <= 0)
            throw new DomainException("İzin günü sayısı sıfırdan büyük olmalıdır.");

        return GetRemainingDays() >= daysRequested;
    }

    /// <summary>
    /// Belirtilen leave type'ı için belirtilen miktar izin alınabilir mi kontrol et
    /// </summary>
    public bool CanTakeLeave(LeaveType leaveType, int daysRequested)
    {
        if (daysRequested <= 0)
            throw new DomainException("İzin günü sayısı sıfırdan büyük olmalıdır.");

        var remaining = GetRemainingDays(leaveType);
        return remaining >= daysRequested;
    }

    /// <summary>
    /// İzin kullan ve yeni bakiye döndür (toplam)
    /// </summary>
    public LeaveBalance UseLeave(int daysUsed)
    {
        if (!CanTakeLeave(daysUsed))
            throw new DomainException(
                $"Yeterli izin günü yok. Kalan: {GetRemainingDays()} gün");

        // ✅ Yıllık izinden kesmeyi dene, sonra raporlu izinden
        var remainingAnnual = AnnualLeaveRemainingDays;
        var remainingSick = SickLeaveRemainingDays;

        int newAnnualUsed = AnnualLeaveUsedDays;
        int newSickUsed = SickLeaveUsedDays;
        int daysLeft = daysUsed;

        if (remainingAnnual > 0)
        {
            int annualToUse = Math.Min(daysLeft, remainingAnnual);
            newAnnualUsed += annualToUse;
            daysLeft -= annualToUse;
        }

        if (daysLeft > 0 && remainingSick > 0)
        {
            int sickToUse = Math.Min(daysLeft, remainingSick);
            newSickUsed += sickToUse;
            daysLeft -= sickToUse;
        }

        return new LeaveBalance(
            AnnualLeaveDays,
            SickLeaveDays,
            CompensatoryLeaveDays,
            newAnnualUsed,
            newSickUsed,
            CompensatoryLeaveUsedDays);
    }

    /// <summary>
    /// İzin kullan ve yeni bakiye döndür (type'a göre)
    /// </summary>
    public LeaveBalance UseLeave(LeaveType leaveType, int daysUsed)
    {
        if (!CanTakeLeave(leaveType, daysUsed))
            throw new DomainException(
                $"Yeterli {leaveType} izin günü yok. " +
                $"Kalan: {GetRemainingDays(leaveType)} gün");

        return leaveType switch
        {
            LeaveType.Annual => new LeaveBalance(
                AnnualLeaveDays,
                SickLeaveDays,
                CompensatoryLeaveDays,
                AnnualLeaveUsedDays + daysUsed,
                SickLeaveUsedDays,
                CompensatoryLeaveUsedDays),

            LeaveType.Sick => new LeaveBalance(
                AnnualLeaveDays,
                SickLeaveDays,
                CompensatoryLeaveDays,
                AnnualLeaveUsedDays,
                SickLeaveUsedDays + daysUsed,
                CompensatoryLeaveUsedDays),

            LeaveType.Compensatory => new LeaveBalance(
                AnnualLeaveDays,
                SickLeaveDays,
                CompensatoryLeaveDays,
                AnnualLeaveUsedDays,
                SickLeaveUsedDays,
                CompensatoryLeaveUsedDays + daysUsed),

            _ => this
        };
    }

    /// <summary>
    /// İzin iade et ve yeni bakiye döndür (toplam)
    /// </summary>
    public LeaveBalance RefundLeave(int daysRefunded)
    {
        int newAnnualUsed = AnnualLeaveUsedDays;
        int newSickUsed = SickLeaveUsedDays;
        int daysLeft = daysRefunded;

        // Önce Sick Leave'den geri al
        if (newSickUsed > 0)
        {
            int sickToRefund = Math.Min(daysLeft, newSickUsed);
            newSickUsed -= sickToRefund;
            daysLeft -= sickToRefund;
        }

        // Sonra Annual Leave'den geri al
        if (daysLeft > 0 && newAnnualUsed > 0)
        {
            int annualToRefund = Math.Min(daysLeft, newAnnualUsed);
            newAnnualUsed -= annualToRefund;
            daysLeft -= annualToRefund;
        }

        return new LeaveBalance(
            AnnualLeaveDays,
            SickLeaveDays,
            CompensatoryLeaveDays,
            newAnnualUsed,
            newSickUsed,
            CompensatoryLeaveUsedDays);
    }

    /// <summary>
    /// İzin iade et ve yeni bakiye döndür (type'a göre)
    /// </summary>
    public LeaveBalance RefundLeave(LeaveType leaveType, int daysRefunded)
    {
        return leaveType switch
        {
            LeaveType.Annual => new LeaveBalance(
                AnnualLeaveDays,
                SickLeaveDays,
                CompensatoryLeaveDays,
                Math.Max(0, AnnualLeaveUsedDays - daysRefunded),
                SickLeaveUsedDays,
                CompensatoryLeaveUsedDays),

            LeaveType.Sick => new LeaveBalance(
                AnnualLeaveDays,
                SickLeaveDays,
                CompensatoryLeaveDays,
                AnnualLeaveUsedDays,
                Math.Max(0, SickLeaveUsedDays - daysRefunded),
                CompensatoryLeaveUsedDays),

            LeaveType.Compensatory => new LeaveBalance(
                AnnualLeaveDays,
                SickLeaveDays,
                CompensatoryLeaveDays,
                AnnualLeaveUsedDays,
                SickLeaveUsedDays,
                Math.Max(0, CompensatoryLeaveUsedDays - daysRefunded)),

            _ => this
        };
    }

    /// <summary>
    /// Yıllık izin bakiyesini güncelle (yıl sonunda)
    /// </summary>
    public LeaveBalance RefreshAnnualLeave(int newAnnualDays)
    {
        return new LeaveBalance(
            newAnnualDays,
            SickLeaveDays,
            CompensatoryLeaveDays,
            0, // ✅ Kullanılan yıllık izini sıfırla
            SickLeaveUsedDays,
            CompensatoryLeaveUsedDays);
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return AnnualLeaveDays;
        yield return SickLeaveDays;
        yield return CompensatoryLeaveDays;
        yield return AnnualLeaveUsedDays;
        yield return SickLeaveUsedDays;
        yield return CompensatoryLeaveUsedDays;
    }
}
