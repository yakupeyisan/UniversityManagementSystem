using UniversityMS.Domain.Entities.Common;
using UniversityMS.Domain.Enums;
using UniversityMS.Domain.Exceptions;

namespace UniversityMS.Domain.Entities.SecurityAggregate;

/// <summary>
/// Kamera (Camera) Entity
/// </summary>
public class Camera : AuditableEntity
{
    public string Code { get; private set; } = null!;
    public string Name { get; private set; } = null!;
    public Guid? AccessPointId { get; private set; }
    public Guid? BuildingId { get; private set; }
    public string Location { get; private set; } = null!;
    public CameraType Type { get; private set; }
    public CameraStatus Status { get; private set; }
    public string? IPAddress { get; private set; }
    public string? StreamUrl { get; private set; }
    public bool IsRecording { get; private set; }
    public bool HasMotionDetection { get; private set; }
    public bool HasFacialRecognition { get; private set; }
    public int? RetentionDays { get; private set; }
    public string? Manufacturer { get; private set; }
    public string? Model { get; private set; }
    public DateTime? LastMaintenanceDate { get; private set; }
    public string? Description { get; private set; }

    // Navigation Properties
    public AccessPoint? AccessPoint { get; private set; }

    private Camera() { }

    private Camera(
        string code,
        string name,
        string location,
        CameraType type,
        Guid? accessPointId = null,
        Guid? buildingId = null)
    {
        Code = code;
        Name = name;
        Location = location;
        Type = type;
        AccessPointId = accessPointId;
        BuildingId = buildingId;
        Status = CameraStatus.Offline;
        IsRecording = false;
        HasMotionDetection = false;
        HasFacialRecognition = false;
        RetentionDays = 30; // Varsayılan 30 gün
    }

    public static Camera Create(
        string code,
        string name,
        string location,
        CameraType type,
        Guid? accessPointId = null,
        Guid? buildingId = null)
    {
        if (string.IsNullOrWhiteSpace(code))
            throw new DomainException("Kamera kodu boş olamaz.");

        if (string.IsNullOrWhiteSpace(name))
            throw new DomainException("Kamera adı boş olamaz.");

        if (string.IsNullOrWhiteSpace(location))
            throw new DomainException("Konum boş olamaz.");

        return new Camera(code, name, location, type, accessPointId, buildingId);
    }

    public void SetOnline()
    {
        Status = CameraStatus.Online;
    }

    public void SetOffline()
    {
        Status = CameraStatus.Offline;
        IsRecording = false;
    }

    public void StartRecording()
    {
        if (Status != CameraStatus.Online)
            throw new DomainException("Sadece çevrimiçi kameralar kayıt yapabilir.");

        Status = CameraStatus.Recording;
        IsRecording = true;
    }

    public void StopRecording()
    {
        if (IsRecording)
        {
            IsRecording = false;
            Status = CameraStatus.Online;
        }
    }

    public void EnableMotionDetection()
    {
        HasMotionDetection = true;
    }

    public void DisableMotionDetection()
    {
        HasMotionDetection = false;
    }

    public void EnableFacialRecognition()
    {
        HasFacialRecognition = true;
    }

    public void DisableFacialRecognition()
    {
        HasFacialRecognition = false;
    }

    public void SetStreamUrl(string streamUrl)
    {
        if (string.IsNullOrWhiteSpace(streamUrl))
            throw new DomainException("Yayın URL'si boş olamaz.");

        StreamUrl = streamUrl;
    }

    public void SetRetentionPeriod(int days)
    {
        if (days <= 0)
            throw new DomainException("Saklama süresi pozitif olmalıdır.");

        RetentionDays = days;
    }

    public void UpdateDeviceInfo(string ipAddress, string? manufacturer = null, string? model = null)
    {
        IPAddress = ipAddress;
        Manufacturer = manufacturer;
        Model = model;
    }

    public void MarkForMaintenance()
    {
        Status = CameraStatus.Maintenance;
    }

    public void CompleteMaintenance()
    {
        LastMaintenanceDate = DateTime.UtcNow;
        Status = CameraStatus.Online;
    }

    public void MarkAsFaulty(string description)
    {
        Status = CameraStatus.Faulty;
        Description = $"Arıza: {description}";
    }

    public bool IsOperational()
    {
        return Status == CameraStatus.Online || Status == CameraStatus.Recording;
    }
}