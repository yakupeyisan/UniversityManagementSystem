using UniversityMS.Domain.Entities.Common;
using UniversityMS.Domain.Enums;

namespace UniversityMS.Domain.Entities.SecurityAggregate;

/// <summary>
/// Güvenlik Kuralı (SecurityRule) - Entity (YENİ)
/// Belirli zamanlara/koşullara dayalı erişim kuralları
/// Örn: Pazartesi-Cuma 08:00-18:00 arasında erişim
/// </summary>
public class SecurityRule : BaseEntity
{
    public Guid SecurityZoneId { get; private set; }
    public string Description { get; private set; } = null!;
    public TimeOnly? AllowedStartTime { get; private set; }
    public TimeOnly? AllowedEndTime { get; private set; }
    public DayOfWeek[]? AllowedDays { get; private set; }
    public AccessLevel MinimumAccessLevel { get; private set; }
    public bool IsActive { get; private set; }

    private SecurityRule() { }

    public SecurityRule(
        Guid securityZoneId,
        string description,
        AccessLevel minimumAccessLevel,
        TimeOnly? startTime = null,
        TimeOnly? endTime = null,
        DayOfWeek[]? allowedDays = null)
    {
        SecurityZoneId = securityZoneId;
        Description = description;
        MinimumAccessLevel = minimumAccessLevel;
        AllowedStartTime = startTime;
        AllowedEndTime = endTime;
        AllowedDays = allowedDays;
        IsActive = true;
    }

    /// <summary>
    /// Erişim kuralına göre izin verme kontrolü yap
    /// </summary>
    public bool IsAccessAllowed(TimeOnly currentTime, DayOfWeek currentDay, AccessLevel userAccessLevel)
    {
        if (!IsActive) return false;
        if (userAccessLevel < MinimumAccessLevel) return false;
        if (AllowedDays != null && !AllowedDays.Contains(currentDay)) return false;
        if (AllowedStartTime.HasValue && currentTime < AllowedStartTime) return false;
        if (AllowedEndTime.HasValue && currentTime > AllowedEndTime) return false;
        return true;
    }

    public void Deactivate() => IsActive = false;
    public void Activate() => IsActive = true;
}