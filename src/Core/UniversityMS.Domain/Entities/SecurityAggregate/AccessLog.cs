using UniversityMS.Domain.Entities.Common;
using UniversityMS.Domain.Entities.PersonAggregate;
using UniversityMS.Domain.Enums;

namespace UniversityMS.Domain.Entities.SecurityAggregate;

/// <summary>
/// Erişim Kaydı (AccessLog) Entity
/// </summary>
public class AccessLog : AuditableEntity
{
    public Guid AccessPointId { get; private set; }
    public Guid? UserId { get; private set; }
    public DateTime AccessTime { get; private set; }
    public AccessLogType Type { get; private set; }
    public AccessResult Result { get; private set; }
    public AccessDenialReason? DenialReason { get; private set; }
    public string Location { get; private set; } = null!;
    public string? CredentialUsed { get; private set; }
    public string? IPAddress { get; private set; }
    public string? DeviceInfo { get; private set; }
    public string? Notes { get; private set; }
    public bool IsAnomaly { get; private set; }

    // Navigation Properties
    public AccessPoint AccessPoint { get; private set; } = null!;
    public Person? User { get; private set; }

    private AccessLog() { }

    private AccessLog(
        Guid accessPointId,
        Guid? userId,
        DateTime accessTime,
        AccessLogType type,
        AccessResult result,
        string location,
        string? credentialUsed = null,
        AccessDenialReason? denialReason = null)
    {
        AccessPointId = accessPointId;
        UserId = userId;
        AccessTime = accessTime;
        Type = type;
        Result = result;
        Location = location;
        CredentialUsed = credentialUsed;
        DenialReason = denialReason;
        IsAnomaly = false;
    }

    public static AccessLog CreateGranted(
        Guid accessPointId,
        Guid userId,
        string location,
        string? credentialUsed = null)
    {
        return new AccessLog(
            accessPointId,
            userId,
            DateTime.UtcNow,
            AccessLogType.Entry,
            AccessResult.Granted,
            location,
            credentialUsed);
    }

    public static AccessLog CreateDenied(
        Guid accessPointId,
        Guid? userId,
        string location,
        AccessDenialReason reason,
        string? credentialUsed = null)
    {
        return new AccessLog(
            accessPointId,
            userId,
            DateTime.UtcNow,
            AccessLogType.Denied,
            AccessResult.Denied,
            location,
            credentialUsed,
            reason);
    }

    public static AccessLog CreateEmergency(
        Guid accessPointId,
        Guid userId,
        string location,
        string reason)
    {
        var log = new AccessLog(
            accessPointId,
            userId,
            DateTime.UtcNow,
            AccessLogType.Emergency,
            AccessResult.Granted,
            location);

        log.Notes = $"Acil durum erişimi: {reason}";
        return log;
    }

    public static AccessLog CreateExit(
        Guid accessPointId,
        Guid userId,
        string location,
        string? credentialUsed = null)
    {
        return new AccessLog(
            accessPointId,
            userId,
            DateTime.UtcNow,
            AccessLogType.Exit,
            AccessResult.Granted,
            location,
            credentialUsed);
    }

    public void MarkAsAnomaly(string reason)
    {
        IsAnomaly = true;
        Notes = $"Anomali: {reason}";
    }

    public void AddNote(string note)
    {
        Notes = string.IsNullOrWhiteSpace(Notes) ? note : $"{Notes}\n{note}";
    }

    public void SetDeviceInfo(string ipAddress, string deviceInfo)
    {
        IPAddress = ipAddress;
        DeviceInfo = deviceInfo;
    }
}