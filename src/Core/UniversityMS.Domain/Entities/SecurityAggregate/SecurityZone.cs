using UniversityMS.Domain.Entities.Common;
using UniversityMS.Domain.Enums;
using UniversityMS.Domain.Exceptions;
using UniversityMS.Domain.Interfaces;

namespace UniversityMS.Domain.Entities.SecurityAggregate;

/// <summary>
/// Güvenlik Bölgesi (SecurityZone) - Aggregate Root (YENİ)
/// Mantıksal güvenlik bölgeleri: Bina, Kat, Bölüm, vb
/// AccessPoint'leri manage etmez, sadece referans tutar
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

    // ⚠️ Ownership yok - sadece referans tutar
    // AccessPoint'ler kendi Aggregate Root'ları, SecurityZone'lar onları manage etmez
    // Repository aracılığıyla "Bana ait AccessPoint'ler nelerdir?" sorusu sorulur

    private readonly List<SecurityRule> _rules = new();
    public IReadOnlyCollection<SecurityRule> Rules => _rules.AsReadOnly();

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

    /// <summary>
    /// Güvenlik kuralı ekle (Zaman bazlı erişim vb)
    /// </summary>
    public void AddSecurityRule(SecurityRule rule)
    {
        if (rule.SecurityZoneId != Id)
            throw new DomainException("Kural bu bölgeye ait değil.");
        _rules.Add(rule);
        AddDomainEvent(new SecurityRuleCreatedEvent(Id, rule.Id));
    }

    /// <summary>
    /// Acil Durum Modu Etkinleştir - Tüm erişim kontrolleri bypass'a alınır
    /// </summary>
    public void ActivateEmergencyMode()
    {
        IsEmergencyModeActive = true;
        AddDomainEvent(new EmergencyModeActivatedEvent(Id, DateTime.UtcNow));
    }

    /// <summary>
    /// Acil Durum Modu Devre Dışı Bırak
    /// </summary>
    public void DeactivateEmergencyMode()
    {
        IsEmergencyModeActive = false;
        AddDomainEvent(new EmergencyModeDeactivatedEvent(Id, DateTime.UtcNow));
    }

    public void UpdateAuditDate()
    {
        LastSecurityAudit = DateTime.UtcNow;
    }

    public void DeactivateZone()
    {
        Status = SecurityZoneStatus.Inactive;
    }

    public void ActivateZone()
    {
        Status = SecurityZoneStatus.Active;
    }
}