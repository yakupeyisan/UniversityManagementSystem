using UniversityMS.Domain.Entities.Common;
using UniversityMS.Domain.Enums;
using UniversityMS.Domain.Events.FacilityEvents;
using UniversityMS.Domain.Exceptions;
using UniversityMS.Domain.ValueObjects;

namespace UniversityMS.Domain.Entities.FacilityAggregate;

/// <summary>
/// Ekipman (Equipment) Entity
/// </summary>
public class Equipment : AuditableEntity
{
    public string Code { get; private set; } = null!;
    public string Name { get; private set; } = null!;
    public EquipmentType Type { get; private set; }
    public EquipmentStatus Status { get; private set; }
    public Guid? BuildingId { get; private set; }
    public Guid? RoomId { get; private set; }
    public Guid? LaboratoryId { get; private set; }
    public string? Manufacturer { get; private set; }
    public string? Model { get; private set; }
    public string? SerialNumber { get; private set; }
    public DateTime? PurchaseDate { get; private set; }
    public Money? PurchasePrice { get; private set; }
    public int? WarrantyMonths { get; private set; }
    public DateTime? WarrantyExpiryDate { get; private set; }
    public DateTime? LastMaintenanceDate { get; private set; }
    public DateTime? NextMaintenanceDate { get; private set; }
    public MaintenanceType? LastMaintenanceType { get; private set; }
    public Guid? AssignedTo { get; private set; }
    public string? Location { get; private set; }
    public string? Description { get; private set; }
    public string? Notes { get; private set; }
    public bool RequiresCalibration { get; private set; }
    public DateTime? LastCalibrationDate { get; private set; }

    // Navigation Properties
    public Building? Building { get; private set; }
    public Room? Room { get; private set; }
    public Laboratory? Laboratory { get; private set; }

    private Equipment() { }

    private Equipment(
        string code,
        string name,
        EquipmentType type,
        Guid? buildingId = null,
        Guid? roomId = null,
        Guid? laboratoryId = null)
    {
        Code = code;
        Name = name;
        Type = type;
        BuildingId = buildingId;
        RoomId = roomId;
        LaboratoryId = laboratoryId;
        Status = EquipmentStatus.Available;
        RequiresCalibration = false;
    }

    public static Equipment Create(
        string code,
        string name,
        EquipmentType type,
        Guid? buildingId = null,
        Guid? roomId = null,
        Guid? laboratoryId = null)
    {
        if (string.IsNullOrWhiteSpace(code))
            throw new DomainException("Ekipman kodu boş olamaz.");

        if (string.IsNullOrWhiteSpace(name))
            throw new DomainException("Ekipman adı boş olamaz.");

        var equipment = new Equipment(code, name, type, buildingId, roomId, laboratoryId);
        equipment.AddDomainEvent(new EquipmentAddedEvent(equipment.Id, code, type, roomId));

        return equipment;
    }

    public void UpdatePurchaseInfo(
        DateTime purchaseDate,
        Money purchasePrice,
        int? warrantyMonths = null,
        string? manufacturer = null,
        string? model = null,
        string? serialNumber = null)
    {
        PurchaseDate = purchaseDate;
        PurchasePrice = purchasePrice;
        WarrantyMonths = warrantyMonths;
        Manufacturer = manufacturer;
        Model = model;
        SerialNumber = serialNumber;

        if (warrantyMonths.HasValue)
            WarrantyExpiryDate = purchaseDate.AddMonths(warrantyMonths.Value);
    }

    public void AssignTo(Guid userId)
    {
        if (Status != EquipmentStatus.Available)
            throw new DomainException("Sadece müsait ekipman atanabilir.");

        AssignedTo = userId;
        Status = EquipmentStatus.InUse;
    }

    public void Return()
    {
        AssignedTo = null;
        Status = EquipmentStatus.Available;
    }

    public void SetUnderMaintenance(MaintenanceType type)
    {
        Status = EquipmentStatus.UnderMaintenance;
        LastMaintenanceType = type;
    }

    public void CompleteMaintenance(Guid completedBy)
    {
        LastMaintenanceDate = DateTime.UtcNow;
        LastMaintenanceType = LastMaintenanceType ?? MaintenanceType.Preventive;
        Status = EquipmentStatus.Available;

        // Bakım tipine göre sonraki bakım tarihini belirle
        var monthsUntilNext = LastMaintenanceType switch
        {
            MaintenanceType.Preventive => 6,
            MaintenanceType.Corrective => 3,
            MaintenanceType.Predictive => 6,
            MaintenanceType.Calibration => 12,
            _ => 6
        };

        NextMaintenanceDate = DateTime.UtcNow.AddMonths(monthsUntilNext);

        AddDomainEvent(new EquipmentMaintenanceCompletedEvent(Id, DateTime.UtcNow, completedBy));
    }

    public void ScheduleMaintenance(DateTime maintenanceDate, MaintenanceType type)
    {
        NextMaintenanceDate = maintenanceDate;

        AddDomainEvent(new EquipmentMaintenanceScheduledEvent(Id, type, maintenanceDate));
    }

    public void MarkAsDamaged(string description)
    {
        Status = EquipmentStatus.Damaged;
        Notes = $"Hasar: {description}";
    }

    public void SetOutOfService(string reason)
    {
        Status = EquipmentStatus.OutOfService;
        Notes = $"Hizmet dışı: {reason}";
    }

    public void Retire(string reason)
    {
        Status = EquipmentStatus.Retired;
        Notes = $"Emekliye ayrılma sebebi: {reason}";
    }

    public void Calibrate()
    {
        if (!RequiresCalibration)
            throw new DomainException("Bu ekipman kalibrasyon gerektirmiyor.");

        LastCalibrationDate = DateTime.UtcNow;
    }

    public void SetCalibrationRequirement(bool required)
    {
        RequiresCalibration = required;
    }

    public void UpdateLocation(string location)
    {
        Location = location;
    }

    public bool IsAvailable()
    {
        return Status == EquipmentStatus.Available;
    }

    public bool NeedsMaintenance()
    {
        return NextMaintenanceDate.HasValue &&
               DateTime.UtcNow >= NextMaintenanceDate.Value;
    }

    public bool IsUnderWarranty()
    {
        return WarrantyExpiryDate.HasValue &&
               DateTime.UtcNow < WarrantyExpiryDate.Value;
    }

    public bool NeedsCalibration()
    {
        if (!RequiresCalibration) return false;

        if (!LastCalibrationDate.HasValue) return true;

        return DateTime.UtcNow >= LastCalibrationDate.Value.AddMonths(12);
    }

    public int GetAgeInMonths()
    {
        if (!PurchaseDate.HasValue) return 0;

        return (int)((DateTime.UtcNow - PurchaseDate.Value).TotalDays / 30.44);
    }
}