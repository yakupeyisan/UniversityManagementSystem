using UniversityMS.Domain.Entities.Common;
using UniversityMS.Domain.Enums;
using UniversityMS.Domain.Events.SecurityEvents;
using UniversityMS.Domain.Exceptions;
using UniversityMS.Domain.Interfaces;
using UniversityMS.Domain.ValueObjects;

namespace UniversityMS.Domain.Entities.SecurityAggregate;


/// <summary>
/// Güvenlik Bölgesi (SecurityZone) - Aggregate Root
/// Mantıksal güvenlik bölgeleri: Bina, Kat, Bölüm, vb
/// - AccessPoint'leri manage etmez, sadece referans tutar
/// - Zaman tabanlı kısıtlamaları manage eder
/// - Bölgeye giriş kurallarını tanımlar
/// </summary>
public class SecurityZone : AuditableEntity, IAggregateRoot
{
    public string Code { get; private set; } = null!;
    public string Name { get; private set; } = null!;
    public string Description { get; private set; } = null!;
    public Guid? CampusId { get; private set; }
    public AccessLevel RequiredAccessLevel { get; private set; }
    public SecurityZoneStatus Status { get; private set; }
    public bool IsEmergencyModeActive { get; private set; }
    public DateTime? LastSecurityAudit { get; private set; }

    // ✅ GÜVENLİK KURALLARI
    private readonly List<SecurityRule> _rules = new();
    public IReadOnlyCollection<SecurityRule> Rules => _rules.AsReadOnly();

    // ✅ ZAMAN KISITLAMALARI (YENİ)
    private readonly List<AccessTimeRestriction> _timeRestrictions = new();
    public IReadOnlyCollection<AccessTimeRestriction> TimeRestrictions => _timeRestrictions.AsReadOnly();

    // ✅ İZİN VERİLEN ERIŞIM SEVİYELERİ (YENİ)
    private readonly List<AccessLevel> _allowedAccessLevels = new();
    public IReadOnlyCollection<AccessLevel> AllowedAccessLevels => _allowedAccessLevels.AsReadOnly();

    private SecurityZone() { }

    private SecurityZone(
        string code,
        string name,
        string description,
        Guid? campusId,
        AccessLevel requiredAccessLevel)
    {
        Code = code;
        Name = name;
        Description = description;
        CampusId = campusId;
        RequiredAccessLevel = requiredAccessLevel;
        Status = SecurityZoneStatus.Active;
        IsEmergencyModeActive = false;

        AddDomainEvent(new SecurityZoneCreatedEvent(Id, Code, Name));
    }

    public static SecurityZone Create(
        string code,
        string name,
        string description,
        Guid? campusId,
        AccessLevel requiredAccessLevel)
    {
        if (string.IsNullOrWhiteSpace(code))
            throw new DomainException("Bölge kodu boş olamaz.");
        if (string.IsNullOrWhiteSpace(name))
            throw new DomainException("Bölge adı boş olamaz.");

        return new SecurityZone(code, name, description, campusId, requiredAccessLevel);
    }

    // ============= SECURITY RULE MANAGEMENT =============

    public void AddSecurityRule(SecurityRule rule)
    {
        if (rule.SecurityZoneId != Id)
            throw new DomainException("Kural bu bölgeye ait değil.");

        _rules.Add(rule);

        AddDomainEvent(new SecurityRuleAddedEvent(Id, rule.Id));
    }

    public void RemoveSecurityRule(Guid ruleId)
    {
        var rule = _rules.FirstOrDefault(r => r.Id == ruleId);
        if (rule == null)
            throw new DomainException("Kural bulunamadı.");

        _rules.Remove(rule);

        AddDomainEvent(new SecurityRuleRemovedEvent(Id, ruleId));
    }

    // ============= ACCESS LEVEL MANAGEMENT =============

    /// <summary>
    /// Bölgeye erişebilecek AccessLevel'ları tanımla
    /// Örn: Student ve Staff erişebilir ama Contractor erişemez
    /// </summary>
    public void AddAllowedAccessLevel(AccessLevel level)
    {
        if (!_allowedAccessLevels.Contains(level))
        {
            _allowedAccessLevels.Add(level);
            AddDomainEvent(new AllowedAccessLevelAddedEvent(Id, level));
        }
    }

    /// <summary>
    /// AccessLevel'ı kaldır
    /// </summary>
    public void RemoveAllowedAccessLevel(AccessLevel level)
    {
        if (_allowedAccessLevels.Contains(level))
        {
            _allowedAccessLevels.Remove(level);
            AddDomainEvent(new AllowedAccessLevelRemovedEvent(Id, level));
        }
    }

    /// <summary>
    /// Tüm AccessLevel'ları izne çıkar
    /// </summary>
    public void AllowAllAccessLevels()
    {
        _allowedAccessLevels.Clear();

        // Tüm seviyeleri ekle
        foreach (var level in Enum.GetValues(typeof(AccessLevel)).Cast<AccessLevel>())
        {
            _allowedAccessLevels.Add(level);
        }
    }

    /// <summary>
    /// Belirli bir AccessLevel'e erişimi kontrol et
    /// </summary>
    public bool IsAccessLevelAllowed(AccessLevel level)
    {
        if (_allowedAccessLevels.Count == 0)
            return true; // Kısıtlama yok

        return _allowedAccessLevels.Contains(level);
    }

    // ============= TIME RESTRICTION MANAGEMENT =============

    /// <summary>
    /// Bölgeye zaman kısıtlaması ekle
    /// </summary>
    public void AddTimeRestriction(AccessTimeRestriction restriction)
    {
        if (restriction.SecurityZoneId != Id)
            throw new DomainException("Kısıtlama bu bölgeye ait değil.");

        _timeRestrictions.Add(restriction);

        AddDomainEvent(new TimeRestrictionAddedEvent(Id, restriction.Id, restriction.Name));
    }

    /// <summary>
    /// Bölgeden zaman kısıtlaması kaldır
    /// </summary>
    public void RemoveTimeRestriction(Guid restrictionId)
    {
        var restriction = _timeRestrictions.FirstOrDefault(r => r.Id == restrictionId);
        if (restriction == null)
            throw new DomainException("Kısıtlama bulunamadı.");

        _timeRestrictions.Remove(restriction);

        AddDomainEvent(new TimeRestrictionRemovedEvent(Id, restrictionId));
    }

    /// <summary>
    /// Belirli bir saatte erişim izni kontrolü
    /// Tüm aktif kısıtlamaları kontrol eder
    /// </summary>
    public bool CanAccessAt(DateTime accessTime, AccessLevel userLevel)
    {
        // AccessLevel kontrol
        if (!IsAccessLevelAllowed(userLevel))
            return false;

        // Zaman kısıtlamaları kontrol
        var activeRestrictions = _timeRestrictions
            .Where(r => r.IsActive)
            .OrderByDescending(r => r.Priority)
            .ToList();

        // Kısıtlama yoksa izin ver
        if (activeRestrictions.Count == 0)
            return true;

        // En yüksek öncelikli kısıtlamayı kontrol et
        var primaryRestriction = activeRestrictions.First();
        return primaryRestriction.IsAccessAllowedAt(accessTime);
    }

    /// <summary>
    /// Şu anda erişim izni var mı?
    /// </summary>
    public bool CanAccessNow(AccessLevel userLevel)
    {
        return CanAccessAt(DateTime.UtcNow, userLevel);
    }

    /// <summary>
    /// Belirli bir tarih aralığında erişim sorunu var mı?
    /// </summary>
    public bool IsAccessBlockedForPeriod(
        DateTime startDate,
        DateTime endDate,
        AccessLevel userLevel)
    {
        if (!IsAccessLevelAllowed(userLevel))
            return true;

        var restrictions = _timeRestrictions
            .Where(r => r.IsActive)
            .OrderByDescending(r => r.Priority)
            .ToList();

        if (restrictions.Count == 0)
            return false;

        var primaryRestriction = restrictions.First();
        return primaryRestriction.IsAccessBlockedForPeriod(startDate, endDate);
    }

    // ============= DURUM YÖNETİMİ =============

    /// <summary>
    /// Acil modu aktifleştir (tüm kısıtlamaları geç)
    /// </summary>
    public void ActivateEmergencyMode(string reason)
    {
        IsEmergencyModeActive = true;
        AddDomainEvent(new EmergencyModeActivatedEvent(Id, Name, reason));
    }

    /// <summary>
    /// Acil modu kapat
    /// </summary>
    public void DeactivateEmergencyMode()
    {
        IsEmergencyModeActive = false;
        AddDomainEvent(new EmergencyModeDeactivatedEvent(Id, Name));
    }

    /// <summary>
    /// Bölgeyi kapat
    /// </summary>
    public void SetStatus(SecurityZoneStatus newStatus)
    {
        if (Status == newStatus)
            return;

        Status = newStatus;
        AddDomainEvent(new SecurityZoneStatusChangedEvent(Id, Status));
    }

    /// <summary>
    /// Güvenlik denetimi
    /// </summary>
    public void PerformSecurityAudit()
    {
        LastSecurityAudit = DateTime.UtcNow;
        AddDomainEvent(new SecurityAuditPerformedEvent(Id, Code));
    }

    // ============= RAPORLAMA =============

    /// <summary>
    /// Bölge yapılandırmasının özet bilgisi
    /// </summary>
    public string GetConfigurationSummary()
    {
        var summary = $"[{Code}] {Name}\n";
        summary += $"Gerekli Erişim Seviyesi: {RequiredAccessLevel}\n";
        summary += $"Durum: {Status}\n";

        if (_allowedAccessLevels.Count > 0)
            summary += $"İzin Verilen Seviyeler: {string.Join(", ", _allowedAccessLevels)}\n";

        if (_timeRestrictions.Count > 0)
        {
            summary += $"Zaman Kısıtlamaları ({_timeRestrictions.Count}):\n";
            foreach (var restriction in _timeRestrictions.Where(r => r.IsActive))
            {
                summary += $"  • {restriction.GetSummary()}\n";
            }
        }

        if (_rules.Count > 0)
            summary += $"Güvenlik Kuralları: {_rules.Count}\n";

        return summary;
    }

    /// <summary>
    /// Erişim izni kontrolü raporu
    /// </summary>
    public AccessCheckReport CheckAccessibility(
        AccessLevel userLevel,
        DateTime targetDate)
    {
        return new AccessCheckReport
        {
            SecurityZoneId = Id,
            ZoneName = Name,
            UserAccessLevel = userLevel,
            TargetDate = targetDate,
            IsAccessLevelAllowed = IsAccessLevelAllowed(userLevel),
            CanAccessAtTime = CanAccessAt(targetDate, userLevel),
            EmergencyModeActive = IsEmergencyModeActive,
            ActiveRestrictions = _timeRestrictions
                .Where(r => r.IsActive)
                .Select(r => r.GetSummary())
                .ToList()
        };
    }
}

public class AccessCheckReport
{
    public Guid SecurityZoneId { get; set; }
    public string ZoneName { get; set; } = null!;
    public AccessLevel UserAccessLevel { get; set; }
    public DateTime TargetDate { get; set; }
    public bool IsAccessLevelAllowed { get; set; }
    public bool CanAccessAtTime { get; set; }
    public bool EmergencyModeActive { get; set; }
    public List<string> ActiveRestrictions { get; set; } = new();

    public bool IsAllowed()
    {
        return (IsAccessLevelAllowed && CanAccessAtTime) || EmergencyModeActive;
    }

    public override string ToString()
    {
        var status = IsAllowed() ? "✅ ERİŞİM İZNİ" : "❌ ERİŞİM YASAĞI";
        return $"{status} - {ZoneName} ({TargetDate:g})";
    }
}

/// <summary>
/// Erişim Zaman Kısıtlaması
/// SecurityZone'lar için detaylı zaman tabanlı kurallar tanımlar
/// - Günlük saat aralıkları
/// - Haftalık pattern'ler
/// - Tatil dönemleri
/// - Özel tarihler
/// - Emergency override
/// </summary>
public class AccessTimeRestriction : AuditableEntity
{
    public Guid SecurityZoneId { get; private set; }
    public string Name { get; private set; } = null!;
    public string Description { get; private set; } = null!;

    // Temel zaman kontrolü
    public AccessTimeType TimeType { get; private set; }
    public TimeSpan? StartTime { get; private set; }          // 09:00
    public TimeSpan? EndTime { get; private set; }            // 17:00

    // Hafta günleri
    private readonly List<DayOfWeek> _allowedDays = new();
    public IReadOnlyCollection<DayOfWeek> AllowedDays => _allowedDays.AsReadOnly();

    // Geçerlilik dönemleri
    public DateTime? ValidFrom { get; private set; }
    public DateTime? ValidUntil { get; private set; }

    // Tatil ve özel tarihler
    private readonly List<DateTime> _holidayDates = new();
    public IReadOnlyCollection<DateTime> HolidayDates => _holidayDates.AsReadOnly();

    private readonly List<DateRange> _holidayPeriods = new();
    public IReadOnlyCollection<DateRange> HolidayPeriods => _holidayPeriods.AsReadOnly();

    // İstisnalar
    private readonly List<DateTime> _exceptionDates = new();
    public IReadOnlyCollection<DateTime> ExceptionDates => _exceptionDates.AsReadOnly();

    // Durum
    public bool IsActive { get; private set; }
    public bool AllowEmergencyOverride { get; private set; }
    public int Priority { get; private set; } // Çakışan kurallar için: 10 = öncelikli

    // Navigation
    public SecurityZone SecurityZone { get; private set; } = null!;

    private AccessTimeRestriction() { }

    private AccessTimeRestriction(
        Guid securityZoneId,
        string name,
        string description,
        AccessTimeType timeType)
    {
        SecurityZoneId = securityZoneId;
        Name = name;
        Description = description;
        TimeType = timeType;
        IsActive = true;
        AllowEmergencyOverride = false;
        Priority = 0;
        ValidFrom = DateTime.UtcNow;
        ValidUntil = null; // Sınırsız
    }

    public static AccessTimeRestriction Create(
        Guid securityZoneId,
        string name,
        string description,
        AccessTimeType timeType)
    {
        if (securityZoneId == Guid.Empty)
            throw new DomainException("SecurityZone ID boş olamaz.");
        if (string.IsNullOrWhiteSpace(name))
            throw new DomainException("Kısıtlama adı boş olamaz.");
        if (string.IsNullOrWhiteSpace(description))
            throw new DomainException("Kısıtlama açıklaması boş olamaz.");

        return new AccessTimeRestriction(securityZoneId, name, description, timeType);
    }

    // ============= TEMEL SAAT KONTROLÜ =============

    /// <summary>
    /// Belirli saatlerde erişim (09:00 - 17:00)
    /// </summary>
    public void SetSpecificHours(TimeSpan startTime, TimeSpan endTime)
    {
        if (startTime >= endTime)
            throw new DomainException("Başlangıç saati bitiş saatinden önce olmalı.");

        TimeType = AccessTimeType.SpecificHours;
        StartTime = startTime;
        EndTime = endTime;
    }

    /// <summary>
    /// İş saatleri (09:00 - 17:00)
    /// </summary>
    public void SetBusinessHours()
    {
        TimeType = AccessTimeType.BusinessHours;
        StartTime = new TimeSpan(9, 0, 0);
        EndTime = new TimeSpan(17, 0, 0);
    }

    /// <summary>
    /// Gece vardiyası (22:00 - 06:00)
    /// </summary>
    public void SetNightShift()
    {
        TimeType = AccessTimeType.NightShift;
        StartTime = new TimeSpan(22, 0, 0);
        EndTime = new TimeSpan(6, 0, 0); // Ertesi gün
    }

    /// <summary>
    /// Tüm gün
    /// </summary>
    public void SetAllDay()
    {
        TimeType = AccessTimeType.AllDay;
        StartTime = null;
        EndTime = null;
    }

    // ============= HAFTA GÜNÜ KONTROLÜ =============

    /// <summary>
    /// İzin verilen günleri ekle (Pazartesi, Salı, vb)
    /// </summary>
    public void AddAllowedDay(DayOfWeek dayOfWeek)
    {
        if (!_allowedDays.Contains(dayOfWeek))
            _allowedDays.Add(dayOfWeek);
    }

    /// <summary>
    /// Hafta içi (Pazartesi - Cuma)
    /// </summary>
    public void SetWeekdaysOnly()
    {
        _allowedDays.Clear();
        TimeType = AccessTimeType.WeekdaysOnly;

        _allowedDays.Add(DayOfWeek.Monday);
        _allowedDays.Add(DayOfWeek.Tuesday);
        _allowedDays.Add(DayOfWeek.Wednesday);
        _allowedDays.Add(DayOfWeek.Thursday);
        _allowedDays.Add(DayOfWeek.Friday);
    }

    /// <summary>
    /// Hafta sonu (Cumartesi - Pazar)
    /// </summary>
    public void SetWeekendsOnly()
    {
        _allowedDays.Clear();
        TimeType = AccessTimeType.WeekendsOnly;

        _allowedDays.Add(DayOfWeek.Saturday);
        _allowedDays.Add(DayOfWeek.Sunday);
    }

    /// <summary>
    /// Tüm günler
    /// </summary>
    public void SetAllDays()
    {
        _allowedDays.Clear();
        _allowedDays.Add(DayOfWeek.Monday);
        _allowedDays.Add(DayOfWeek.Tuesday);
        _allowedDays.Add(DayOfWeek.Wednesday);
        _allowedDays.Add(DayOfWeek.Thursday);
        _allowedDays.Add(DayOfWeek.Friday);
        _allowedDays.Add(DayOfWeek.Saturday);
        _allowedDays.Add(DayOfWeek.Sunday);
    }

    /// <summary>
    /// Günü izin verilen günlerden kaldır
    /// </summary>
    public void RemoveAllowedDay(DayOfWeek dayOfWeek)
    {
        _allowedDays.Remove(dayOfWeek);
    }

    // ============= GEÇERLİLİK DÖNEMİ =============

    /// <summary>
    /// Kuralın geçerli olacağı dönemi belirle (örn: Güz 2024)
    /// </summary>
    public void SetValidityPeriod(DateTime validFrom, DateTime? validUntil = null)
    {
        if (validFrom > (validUntil ?? DateTime.MaxValue))
            throw new DomainException("Başlangıç tarihi bitiş tarihinden önce olmalı.");

        ValidFrom = validFrom;
        ValidUntil = validUntil;
    }

    // ============= TATİL YÖNETİMİ =============

    /// <summary>
    /// Tekil tatil tarihi ekle (15 Ağustos, 23 Nisan, vb)
    /// </summary>
    public void AddHolidayDate(DateTime holidayDate)
    {
        var dateOnly = holidayDate.Date;
        if (!_holidayDates.Any(h => h.Date == dateOnly))
            _holidayDates.Add(dateOnly);
    }

    /// <summary>
    /// Tatil dönemini ekle (Yarıyıl tatili: 01.02.2025 - 16.02.2025)
    /// </summary>
    public void AddHolidayPeriod(DateTime startDate, DateTime endDate, string name)
    {
        if (startDate > endDate)
            throw new DomainException("Başlangıç tarihi bitiş tarihinden önce olmalı.");

        var period = DateRange.Create(startDate, endDate, name);
        _holidayPeriods.Add(period);
    }

    /// <summary>
    /// Tekil tatil tarihini kaldır
    /// </summary>
    public void RemoveHolidayDate(DateTime holidayDate)
    {
        _holidayDates.RemoveAll(h => h.Date == holidayDate.Date);
    }

    /// <summary>
    /// Tatil dönemini kaldır
    /// </summary>
    public void RemoveHolidayPeriod(Guid periodId)
    {
        var period = _holidayPeriods.FirstOrDefault(p => p.Id == periodId);
        if (period != null)
            _holidayPeriods.Remove(period);
    }

    // ============= İSTİSNA TARİHLERİ =============

    /// <summary>
    /// Tatil olmasına rağmen erişim izin ver (Acil durum yönetimi)
    /// </summary>
    public void AddExceptionDate(DateTime exceptionDate, string reason)
    {
        var dateOnly = exceptionDate.Date;
        if (!_exceptionDates.Any(e => e.Date == dateOnly))
        {
            _exceptionDates.Add(dateOnly);
        }
    }

    /// <summary>
    /// İstisna tarihini kaldır
    /// </summary>
    public void RemoveExceptionDate(DateTime exceptionDate)
    {
        _exceptionDates.RemoveAll(e => e.Date == exceptionDate.Date);
    }

    // ============= ERİŞİM KONTROLÜ =============

    /// <summary>
    /// Belirtilen saatte erişim izni var mı?
    /// </summary>
    public bool IsAccessAllowedAt(DateTime accessDateTime)
    {
        // Geçerlilik dönemi kontrol
        if (ValidFrom.HasValue && accessDateTime < ValidFrom)
            return false;
        if (ValidUntil.HasValue && accessDateTime > ValidUntil)
            return false;

        var accessDate = accessDateTime.Date;

        // İstisna tarihleri kontrol (tatil olsa da geç)
        if (_exceptionDates.Contains(accessDate))
            return true;

        // Tekil tatil tarihi kontrol
        if (_holidayDates.Contains(accessDate))
            return false;

        // Tatil dönemini kontrol
        if (_holidayPeriods.Any(p => p.IsInRange(accessDate)))
            return false;

        // Gün kontrolü
        if (_allowedDays.Count > 0 && !_allowedDays.Contains(accessDateTime.DayOfWeek))
            return false;

        // Saat kontrolü
        if (TimeType == AccessTimeType.SpecificHours && StartTime.HasValue && EndTime.HasValue)
        {
            var timeOfDay = accessDateTime.TimeOfDay;
            return timeOfDay >= StartTime && timeOfDay < EndTime;
        }

        if (TimeType == AccessTimeType.BusinessHours)
            return accessDateTime.Hour >= 9 && accessDateTime.Hour < 17;

        if (TimeType == AccessTimeType.AfterHours)
            return accessDateTime.Hour < 9 || accessDateTime.Hour >= 17;

        if (TimeType == AccessTimeType.NightShift)
            return accessDateTime.Hour >= 22 || accessDateTime.Hour < 6;

        // Diğer tüm durumlarda izin ver
        return true;
    }

    /// <summary>
    /// İlgili bir tarih aralığında erişim engeli var mı kontrol et
    /// </summary>
    public bool IsAccessBlockedForPeriod(DateTime startDate, DateTime endDate)
    {
        if (startDate > endDate)
            return false;

        var current = startDate.Date;
        while (current <= endDate.Date)
        {
            if (!IsAccessAllowedAt(current))
                return true;

            current = current.AddDays(1);
        }

        return false;
    }

    // ============= DURUM YÖNETİMİ =============

    /// <summary>
    /// Kısıtlamayı aktif et
    /// </summary>
    public void Activate()
    {
        IsActive = true;
    }

    /// <summary>
    /// Kısıtlamayı deaktif et
    /// </summary>
    public void Deactivate()
    {
        IsActive = false;
    }

    /// <summary>
    /// Acil durum durumunda override izni ver
    /// </summary>
    public void AllowEmergencyAccess()
    {
        AllowEmergencyOverride = true;
    }

    /// <summary>
    /// Emergency override'ı kapat
    /// </summary>
    public void DisallowEmergencyAccess()
    {
        AllowEmergencyOverride = false;
    }

    /// <summary>
    /// Öncelik belirle (çakışan kurallar için)
    /// </summary>
    public void SetPriority(int priority)
    {
        if (priority < 0)
            throw new DomainException("Öncelik negatif olamaz.");

        Priority = priority;
    }

    // ============= RAPORLAMA =============

    /// <summary>
    /// Kısıtlamanın özet bilgisini getir
    /// </summary>
    public string GetSummary()
    {
        var summary = $"[{Name}] ";

        // Saat bilgisi
        if (TimeType == AccessTimeType.SpecificHours && StartTime.HasValue && EndTime.HasValue)
            summary += $"{StartTime:hh\\:mm} - {EndTime:hh\\:mm} | ";
        else
            summary += $"{TimeType} | ";

        // Gün bilgisi
        if (_allowedDays.Count > 0 && _allowedDays.Count < 7)
            summary += $"{string.Join(", ", _allowedDays.Take(3))} | ";

        // Tatil bilgisi
        if (_holidayDates.Count > 0)
            summary += $"Tatil: {_holidayDates.Count} gün | ";

        if (_holidayPeriods.Count > 0)
            summary += $"Tatil dönemleri: {_holidayPeriods.Count} | ";

        // Geçerlilik bilgisi
        if (ValidUntil.HasValue)
            summary += $"Geçerli: {ValidFrom:d} - {ValidUntil:d}";

        return summary;
    }

    /// <summary>
    /// Kullanıcı dostu metin
    /// </summary>
    public string GetUserFriendlyDescription()
    {
        return Description;
    }
}
