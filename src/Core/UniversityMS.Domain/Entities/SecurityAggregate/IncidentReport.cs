using UniversityMS.Domain.Entities.Common;
using UniversityMS.Domain.Enums;
using UniversityMS.Domain.Events.SecurityEvents;
using UniversityMS.Domain.Exceptions;
using UniversityMS.Domain.Interfaces;

namespace UniversityMS.Domain.Entities.SecurityAggregate;

/// <summary>
/// Olay Raporu (IncidentReport) - Aggregate Root (YENİ)
/// Güvenlik olaylarını ve anomalileri kaydetme
/// </summary>
public class IncidentReport : AuditableEntity, IAggregateRoot
{
    public string ReportNumber { get; private set; } = null!;
    public Guid? SecurityZoneId { get; private set; } // Reference (ownership değil)
    public IncidentType Type { get; private set; }
    public IncidentSeverity Severity { get; private set; }
    public string Description { get; private set; } = null!;
    public DateTime IncidentTime { get; private set; }
    public IncidentStatus Status { get; private set; }
    public Guid? ReportedBy { get; private set; }
    public Guid? InvestigatedBy { get; private set; }
    public string? Findings { get; private set; }
    public string? ResolutionNotes { get; private set; }
    public DateTime? ResolvedAt { get; private set; }

    private IncidentReport() { }

    public static IncidentReport Create(
        IncidentType type,
        IncidentSeverity severity,
        string description,
        DateTime incidentTime,
        Guid? securityZoneId = null,
        Guid? reportedBy = null)
    {
        if (string.IsNullOrWhiteSpace(description))
            throw new DomainException("Açıklama boş olamaz.");

        var report = new IncidentReport
        {
            SecurityZoneId = securityZoneId,
            Type = type,
            Severity = severity,
            Description = description,
            IncidentTime = incidentTime,
            ReportedBy = reportedBy,
            Status = IncidentStatus.Reported
        };

        report.GenerateReportNumber();
        report.AddDomainEvent(new IncidentReportedEvent(report.Id, securityZoneId, severity));
        return report;
    }

    private void GenerateReportNumber()
    {
        ReportNumber = $"INC{DateTime.Now:yyyyMMddHHmmss}";
    }

    public void AssignInvestigator(Guid investigatorId)
    {
        InvestigatedBy = investigatorId;
        Status = IncidentStatus.Investigating;
    }

    public void ResolveIncident(string findings, string resolutionNotes)
    {
        if (Status == IncidentStatus.Resolved)
            throw new DomainException("Bu olay zaten çözüldü.");

        Findings = findings;
        ResolutionNotes = resolutionNotes;
        Status = IncidentStatus.Resolved;
        ResolvedAt = DateTime.UtcNow;
        AddDomainEvent(new IncidentResolvedEvent(Id, SecurityZoneId));
    }

    public void CancelIncident()
    {
        if (Status == IncidentStatus.Resolved || Status == IncidentStatus.Closed)
            throw new DomainException("Çözülen veya kapatılan olay iptal edilemez.");

        Status = IncidentStatus.Closed;
    }
}