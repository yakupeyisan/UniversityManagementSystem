using UniversityMS.Domain.Entities.Common;
using UniversityMS.Domain.Entities.PersonAggregate;
using UniversityMS.Domain.Enums;
using UniversityMS.Domain.Events.SecurityEvents;
using UniversityMS.Domain.Exceptions;

namespace UniversityMS.Domain.Entities.SecurityAggregate;

/// <summary>
/// Acil Durum Alarmı (EmergencyAlert) Entity
/// </summary>
public class EmergencyAlert : AuditableEntity
{
    public string AlertCode { get; private set; } = null!;
    public EmergencyAlertType Type { get; private set; }
    public EmergencyAlertStatus Status { get; private set; }
    public EmergencyPriority Priority { get; private set; }
    public string Location { get; private set; } = null!;
    public Guid? BuildingId { get; private set; }
    public Guid? CampusId { get; private set; }
    public DateTime TriggeredAt { get; private set; }
    public Guid? TriggeredBy { get; private set; }
    public DateTime? AcknowledgedAt { get; private set; }
    public Guid? AcknowledgedBy { get; private set; }
    public DateTime? ResolvedAt { get; private set; }
    public Guid? ResolvedBy { get; private set; }
    public string Description { get; private set; } = null!;
    public string? ResponseNotes { get; private set; }
    public int? AffectedPersonCount { get; private set; }
    public bool IsActive { get; private set; }
    public string? ResolutionDetails { get; private set; }

    // Navigation Properties
    public Person? Triggerer { get; private set; }
    public Person? Acknowledger { get; private set; }
    public Person? Resolver { get; private set; }

    private EmergencyAlert() { }

    private EmergencyAlert(
        string alertCode,
        EmergencyAlertType type,
        EmergencyPriority priority,
        string location,
        string description,
        Guid? buildingId = null,
        Guid? campusId = null,
        Guid? triggeredBy = null)
    {
        AlertCode = alertCode;
        Type = type;
        Priority = priority;
        Location = location;
        Description = description;
        BuildingId = buildingId;
        CampusId = campusId;
        TriggeredBy = triggeredBy;
        TriggeredAt = DateTime.UtcNow;
        Status = EmergencyAlertStatus.Active;
        IsActive = true;
    }

    public static EmergencyAlert Create(
        string alertCode,
        EmergencyAlertType type,
        EmergencyPriority priority,
        string location,
        string description,
        Guid? buildingId = null,
        Guid? campusId = null,
        Guid? triggeredBy = null)
    {
        if (string.IsNullOrWhiteSpace(alertCode))
            throw new DomainException("Alarm kodu boş olamaz.");

        if (string.IsNullOrWhiteSpace(location))
            throw new DomainException("Konum boş olamaz.");

        if (string.IsNullOrWhiteSpace(description))
            throw new DomainException("Açıklama boş olamaz.");

        var alert = new EmergencyAlert(alertCode, type, priority, location, description, buildingId, campusId, triggeredBy);
        alert.AddDomainEvent(new EmergencyAlertTriggeredEvent(alert.Id, type, location, priority, alert.TriggeredAt));

        return alert;
    }

    public void Acknowledge(Guid acknowledgedBy, string? notes = null)
    {
        if (Status != EmergencyAlertStatus.Active)
            throw new DomainException("Sadece aktif alarmlar onaylanabilir.");

        Status = EmergencyAlertStatus.Acknowledged;
        AcknowledgedAt = DateTime.UtcNow;
        AcknowledgedBy = acknowledgedBy;

        if (!string.IsNullOrWhiteSpace(notes))
            ResponseNotes = notes;
    }

    public void StartResponse(string notes)
    {
        if (Status != EmergencyAlertStatus.Acknowledged)
            throw new DomainException("Alarm önce onaylanmalıdır.");

        Status = EmergencyAlertStatus.Responding;
        ResponseNotes = string.IsNullOrWhiteSpace(ResponseNotes)
            ? notes
            : $"{ResponseNotes}\n{notes}";
    }

    public void Resolve(Guid resolvedBy, string resolutionDetails, bool isFalseAlarm = false)
    {
        if (Status == EmergencyAlertStatus.Resolved)
            throw new DomainException("Alarm zaten çözülmüş.");

        if (string.IsNullOrWhiteSpace(resolutionDetails))
            throw new DomainException("Çözüm detayları belirtilmelidir.");

        Status = isFalseAlarm ? EmergencyAlertStatus.FalseAlarm : EmergencyAlertStatus.Resolved;
        ResolvedAt = DateTime.UtcNow;
        ResolvedBy = resolvedBy;
        ResolutionDetails = resolutionDetails;
        IsActive = false;

        AddDomainEvent(new EmergencyAlertResolvedEvent(Id, resolvedBy, ResolvedAt.Value));
    }

    public void Cancel(string reason)
    {
        if (Status == EmergencyAlertStatus.Resolved)
            throw new DomainException("Çözülmüş alarm iptal edilemez.");

        Status = EmergencyAlertStatus.Cancelled;
        ResolutionDetails = $"İptal sebebi: {reason}";
        IsActive = false;
    }

    public void UpdateAffectedCount(int count)
    {
        if (count < 0)
            throw new DomainException("Etkilenen kişi sayısı negatif olamaz.");

        AffectedPersonCount = count;
    }

    public void AddResponseNote(string note)
    {
        ResponseNotes = string.IsNullOrWhiteSpace(ResponseNotes)
            ? note
            : $"{ResponseNotes}\n{DateTime.UtcNow:yyyy-MM-dd HH:mm}: {note}";
    }

    public TimeSpan GetResponseTime()
    {
        if (!AcknowledgedAt.HasValue)
            return TimeSpan.Zero;

        return AcknowledgedAt.Value - TriggeredAt;
    }

    public TimeSpan GetResolutionTime()
    {
        if (!ResolvedAt.HasValue)
            return TimeSpan.Zero;

        return ResolvedAt.Value - TriggeredAt;
    }

    public bool IsOverdue()
    {
        // Kritik alarmlar 5 dakika, yüksek öncelikli 15 dakika içinde cevaplanmalı
        var threshold = Priority == EmergencyPriority.Critical ? 5 : 15;
        var elapsed = DateTime.UtcNow - TriggeredAt;

        return Status == EmergencyAlertStatus.Active && elapsed.TotalMinutes > threshold;
    }
}