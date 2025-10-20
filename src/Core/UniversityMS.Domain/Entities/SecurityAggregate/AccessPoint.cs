using UniversityMS.Domain.Entities.Common;
using UniversityMS.Domain.Enums;
using UniversityMS.Domain.Events.SecurityEvents;
using UniversityMS.Domain.Exceptions;
using UniversityMS.Domain.Interfaces;

namespace UniversityMS.Domain.Entities.SecurityAggregate;

/// <summary>
/// Erişim Noktası (AccessPoint) - Aggregate Root
/// Giriş/Çıkış kontrol noktalarını temsil eder
/// </summary>
public class AccessPoint : AuditableEntity, IAggregateRoot
{
    public string Code { get; private set; } = null!;
    public string Name { get; private set; } = null!;
    public string Location { get; private set; } = null!;
    public Guid? BuildingId { get; private set; }
    public Guid? CampusId { get; private set; }
    public AccessPointType Type { get; private set; }
    public AccessPointStatus Status { get; private set; }
    public string? IPAddress { get; private set; }
    public string? DeviceId { get; private set; }
    public bool RequiresTwoFactorAuth { get; private set; }
    public bool IsEmergencyExit { get; private set; }
    public TimeOnly? AccessStartTime { get; private set; }
    public TimeOnly? AccessEndTime { get; private set; }
    public string? Description { get; private set; }
    public DateTime? LastMaintenanceDate { get; private set; }
    public DateTime? NextMaintenanceDate { get; private set; }
    public Guid? SecurityZoneId { get; private set; } 

    // Collections
    private readonly List<AccessLog> _accessLogs = new();
    public IReadOnlyCollection<AccessLog> AccessLogs => _accessLogs.AsReadOnly();

    private readonly List<Camera> _cameras = new();
    public IReadOnlyCollection<Camera> Cameras => _cameras.AsReadOnly();

    private AccessPoint() { }

    private AccessPoint(
        string code,
        string name,
        string location,
        AccessPointType type,
        Guid? buildingId = null,
        Guid? campusId = null,
        bool isEmergencyExit = false)
    {
        Code = code;
        Name = name;
        Location = location;
        Type = type;
        BuildingId = buildingId;
        CampusId = campusId;
        Status = AccessPointStatus.Inactive;
        IsEmergencyExit = isEmergencyExit;
        RequiresTwoFactorAuth = false;
    }

    public static AccessPoint Create(
        string code,
        string name,
        string location,
        AccessPointType type,
        Guid? buildingId = null,
        Guid? campusId = null,
        bool isEmergencyExit = false)
    {
        if (string.IsNullOrWhiteSpace(code))
            throw new DomainException("Erişim noktası kodu boş olamaz.");

        if (string.IsNullOrWhiteSpace(name))
            throw new DomainException("Erişim noktası adı boş olamaz.");

        if (string.IsNullOrWhiteSpace(location))
            throw new DomainException("Konum boş olamaz.");

        return new AccessPoint(code, name, location, type, buildingId, campusId, isEmergencyExit);
    }

    #region Configuration

    public void Activate()
    {
        if (Status == AccessPointStatus.Faulty)
            throw new DomainException("Arızalı erişim noktası aktive edilemez.");

        Status = AccessPointStatus.Active;
        AddDomainEvent(new AccessPointActivatedEvent(Id, Name, Location));
    }

    public void Deactivate(string reason)
    {
        Status = AccessPointStatus.Inactive;
        AddDomainEvent(new AccessPointDeactivatedEvent(Id, reason));
    }

    public void SetMaintenanceMode()
    {
        Status = AccessPointStatus.Maintenance;
    }

    public void MarkAsFaulty(string description)
    {
        Status = AccessPointStatus.Faulty;
        Description = $"Arıza: {description}";
    }
    public void AssignToSecurityZone(Guid securityZoneId)
    {
        SecurityZoneId = securityZoneId;
        AddDomainEvent(new AccessPointAssignedToZoneEvent(Id, securityZoneId));
    }
    public void UnassignFromSecurityZone()
    {
        SecurityZoneId = null;
    }

    public void SetAccessHours(TimeOnly startTime, TimeOnly endTime)
    {
        if (endTime <= startTime)
            throw new DomainException("Bitiş saati başlangıç saatinden sonra olmalıdır.");

        AccessStartTime = startTime;
        AccessEndTime = endTime;
    }

    public void RemoveAccessHours()
    {
        AccessStartTime = null;
        AccessEndTime = null;
    }

    public void SetTwoFactorAuth(bool required)
    {
        RequiresTwoFactorAuth = required;
    }

    public void UpdateDeviceInfo(string? ipAddress, string? deviceId)
    {
        IPAddress = ipAddress;
        DeviceId = deviceId;
    }

    public void ScheduleMaintenance(DateTime maintenanceDate)
    {
        NextMaintenanceDate = maintenanceDate;
    }

    public void CompleteMaintenance()
    {
        LastMaintenanceDate = DateTime.UtcNow;
        NextMaintenanceDate = DateTime.UtcNow.AddMonths(6); // 6 aylık bakım periyodu

        if (Status == AccessPointStatus.Maintenance)
            Status = AccessPointStatus.Active;
    }

    #endregion

    #region Access Control

    public AccessLog GrantAccess(Guid userId, string? credentialUsed = null)
    {
        if (Status != AccessPointStatus.Active)
            throw new DomainException("Erişim noktası aktif değil.");

        if (!IsAccessAllowedAtCurrentTime())
            throw new DomainException("Bu saatte erişime izin verilmiyor.");

        var accessLog = AccessLog.CreateGranted(
            Id,
            userId,
            Location,
            credentialUsed);

        _accessLogs.Add(accessLog);

        AddDomainEvent(new AccessGrantedEvent(Id, userId, Name, DateTime.UtcNow));

        return accessLog;
    }

    public AccessLog DenyAccess(
        Guid? userId,
        AccessDenialReason reason,
        string? credentialUsed = null)
    {
        var accessLog = AccessLog.CreateDenied(
            Id,
            userId,
            Location,
            reason,
            credentialUsed);

        _accessLogs.Add(accessLog);

        AddDomainEvent(new AccessDeniedEvent(Id, userId, Name, reason, DateTime.UtcNow));

        return accessLog;
    }

    public AccessLog RecordEmergencyAccess(Guid userId, string reason)
    {
        var accessLog = AccessLog.CreateEmergency(Id, userId, Location, reason);
        _accessLogs.Add(accessLog);

        return accessLog;
    }

    public void EmergencyUnlock()
    {
        Status = AccessPointStatus.EmergencyOpen;
    }

    public void EmergencyLock()
    {
        Status = AccessPointStatus.Locked;
    }

    #endregion

    #region Camera Management

    public void AddCamera(Camera camera)
    {
        if (camera.AccessPointId != Id)
            throw new DomainException("Kamera bu erişim noktasına ait değil.");

        _cameras.Add(camera);
    }

    public void RemoveCamera(Guid cameraId)
    {
        var camera = _cameras.FirstOrDefault(c => c.Id == cameraId);
        if (camera != null)
            _cameras.Remove(camera);
    }

    #endregion

    #region Helper Methods

    public bool IsAccessAllowedAtCurrentTime()
    {
        // Eğer saat kısıtlaması yoksa her zaman izin verilir
        if (!AccessStartTime.HasValue || !AccessEndTime.HasValue)
            return true;

        var currentTime = TimeOnly.FromDateTime(DateTime.Now);
        return currentTime >= AccessStartTime.Value && currentTime <= AccessEndTime.Value;
    }

    public bool IsOperational()
    {
        return Status == AccessPointStatus.Active;
    }

    public bool NeedsMaintenance()
    {
        return NextMaintenanceDate.HasValue &&
               DateTime.UtcNow >= NextMaintenanceDate.Value;
    }

    public int GetTodayAccessCount()
    {
        var today = DateOnly.FromDateTime(DateTime.UtcNow);
        return _accessLogs.Count(l =>
            DateOnly.FromDateTime(l.AccessTime) == today &&
            l.Result == AccessResult.Granted);
    }

    public int GetDeniedAccessCountToday()
    {
        var today = DateOnly.FromDateTime(DateTime.UtcNow);
        return _accessLogs.Count(l =>
            DateOnly.FromDateTime(l.AccessTime) == today &&
            l.Result == AccessResult.Denied);
    }

    #endregion
}