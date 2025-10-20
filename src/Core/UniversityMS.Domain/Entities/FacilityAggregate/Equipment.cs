using UniversityMS.Domain.Entities.Common;
using UniversityMS.Domain.Enums;
using UniversityMS.Domain.Events.FacilityEvents;
using UniversityMS.Domain.Exceptions;
using UniversityMS.Domain.ValueObjects;

namespace UniversityMS.Domain.Entities.FacilityAggregate;

/// <summary>
/// Ekipman (Equipment) Entity
/// Binalar, odalar, laboratuvarlarda bulunan ekipmanları yönetir
/// </summary>
public class Equipment : AuditableEntity
{
    // ========== Properties ==========
    public string Code { get; private set; } = null!;
    public string Name { get; private set; } = null!;
    public EquipmentType Type { get; private set; }
    public EquipmentStatus Status { get; private set; }

    // ========== Location Info ==========
    public Guid? BuildingId { get; private set; }
    public Guid? RoomId { get; private set; }
    public Guid? LaboratoryId { get; private set; }
    public string? Location { get; private set; }

    // ========== Hardware Details ==========
    public string? Manufacturer { get; private set; }
    public string? Model { get; private set; }
    public string? SerialNumber { get; private set; }

    // ========== Purchase Info ==========
    public DateTime? PurchaseDate { get; private set; }
    public Money? PurchasePrice { get; private set; }
    public int? WarrantyMonths { get; private set; }
    public DateTime? WarrantyExpiryDate { get; private set; }

    // ========== Maintenance Info ==========
    public DateTime? LastMaintenanceDate { get; private set; }
    public DateTime? NextMaintenanceDate { get; private set; }
    public EquipmentMaintenanceType? LastMaintenanceType { get; private set; }

    // ========== Calibration Info ==========
    public bool RequiresCalibration { get; private set; }
    public DateTime? LastCalibrationDate { get; private set; }

    // ========== Usage Info ==========
    public Guid? AssignedTo { get; private set; }

    // ========== Metadata ==========
    public string? Description { get; private set; }
    public string? Notes { get; private set; }

    // ========== Navigation Properties ==========
    public Building? Building { get; private set; }
    public Room? Room { get; private set; }
    public Laboratory? Laboratory { get; private set; }

    // ========== Constructor ==========
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
        Status = EquipmentStatus.Operational;  // ✅ Available → Operational
        RequiresCalibration = false;
    }

    // ========== Factory Method ==========
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

    // ========== Business Methods ==========

    /// <summary>
    /// Satın alma bilgilerini güncelle
    /// </summary>
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

        // Garanti süresi hesapla
        if (warrantyMonths.HasValue)
            WarrantyExpiryDate = purchaseDate.AddMonths(warrantyMonths.Value);

        AddDomainEvent(new EquipmentUpdatedEvent(Id, manufacturer, model, serialNumber));
    }

    /// <summary>
    /// Ekipmanı bir kullanıcıya ata
    /// </summary>
    public void AssignTo(Guid userId)
    {
        if (Status != EquipmentStatus.Operational && Status != EquipmentStatus.Reserved)
            throw new DomainException("Sadece müsait ekipman atanabilir.");

        AssignedTo = userId;
        Status = EquipmentStatus.Reserved;  // ✅ InUse → Reserved

        AddDomainEvent(new EquipmentAssignedEvent(Id, userId, DateTime.UtcNow));
    }

    /// <summary>
    /// Ekipmanı iade et (atanmış durumdan çıkar)
    /// </summary>
    public void Return()
    {
        if (AssignedTo == null)
            throw new DomainException("Bu ekipman atanmış durumda değil.");

        AssignedTo = null;
        Status = EquipmentStatus.Operational;  // ✅ Available → Operational

        AddDomainEvent(new EquipmentReturnedEvent(Id, DateTime.UtcNow));
    }

    /// <summary>
    /// Ekipmanı bakıma al
    /// </summary>
    public void SetUnderMaintenance(EquipmentMaintenanceType type)  // ✅ MaintenanceType → EquipmentMaintenanceType
    {
        Status = EquipmentStatus.InRepair;  // ✅ UnderMaintenance → InRepair
        LastMaintenanceType = type;

        AddDomainEvent(new EquipmentMaintenanceStartedEvent(Id, type, DateTime.UtcNow));
    }

    /// <summary>
    /// Bakımı tamamla
    /// </summary>
    public void CompleteMaintenance(Guid completedBy)
    {
        if (Status != EquipmentStatus.InRepair && Status != EquipmentStatus.Calibrating)
            throw new DomainException("Bu ekipman bakımda değil.");

        LastMaintenanceDate = DateTime.UtcNow;
        LastMaintenanceType = LastMaintenanceType ?? EquipmentMaintenanceType.Preventive;
        Status = EquipmentStatus.Operational;

        // ✅ Bakım tipine göre sonraki bakım tarihini belirle
        var monthsUntilNext = LastMaintenanceType switch
        {
            EquipmentMaintenanceType.Preventive => 6,
            EquipmentMaintenanceType.Corrective => 3,
            EquipmentMaintenanceType.Inspection => 6,
            EquipmentMaintenanceType.Calibration => 12,
            EquipmentMaintenanceType.Repair => 3,
            EquipmentMaintenanceType.Cleaning => 1,
            EquipmentMaintenanceType.Upgrade => 12,
            _ => 6
        };

        NextMaintenanceDate = DateTime.UtcNow.AddMonths(monthsUntilNext);

        AddDomainEvent(new EquipmentMaintenanceCompletedEvent(Id, DateTime.UtcNow, completedBy));
    }

    /// <summary>
    /// Bakım takvimi oluştur
    /// </summary>
    public void ScheduleMaintenance(DateTime maintenanceDate, EquipmentMaintenanceType type)
    {
        if (maintenanceDate <= DateTime.UtcNow)
            throw new DomainException("Bakım tarihi gelecekte olmalıdır.");

        NextMaintenanceDate = maintenanceDate;

        AddDomainEvent(new EquipmentMaintenanceScheduledEvent(Id, type, maintenanceDate));
    }

    /// <summary>
    /// Ekipmanı hasarlı olarak işaretle
    /// </summary>
    public void MarkAsDamaged(string description)
    {
        Status = EquipmentStatus.NeedsRepair;  // ✅ Damaged → NeedsRepair
        Notes = $"Hasar: {description}";

        AddDomainEvent(new EquipmentDamagedEvent(Id, description, DateTime.UtcNow));
    }

    /// <summary>
    /// Ekipmanı hizmet dışı olarak işaretle
    /// </summary>
    public void SetOutOfService(string reason)
    {
        Status = EquipmentStatus.OutOfService;
        Notes = $"Hizmet dışı: {reason}";

        AddDomainEvent(new EquipmentOutOfServiceEvent(Id, reason, DateTime.UtcNow));
    }

    /// <summary>
    /// Ekipmanı emekliye ayır (hizmetinden çek)
    /// </summary>
    public void Retire(string reason)
    {
        Status = EquipmentStatus.Decommissioned;  // ✅ Retired → Decommissioned
        Notes = $"Emekliye ayrılma sebebi: {reason}";

        AddDomainEvent(new EquipmentRetiredEvent(Id, reason, DateTime.UtcNow));
    }

    /// <summary>
    /// Kalibrasyon işlemi yap
    /// </summary>
    public void Calibrate()
    {
        if (!RequiresCalibration)
            throw new DomainException("Bu ekipman kalibrasyon gerektirmiyor.");

        LastCalibrationDate = DateTime.UtcNow;
        Status = EquipmentStatus.Operational;

        // Sonraki kalibrasyon tarihi: 12 ay sonra
        var nextCalibration = DateTime.UtcNow.AddMonths(12);
        if (!NextMaintenanceDate.HasValue || NextMaintenanceDate > nextCalibration)
            NextMaintenanceDate = nextCalibration;

        AddDomainEvent(new EquipmentCalibratedEvent(Id, DateTime.UtcNow));
    }

    /// <summary>
    /// Kalibrasyon gereksinimi belirle
    /// </summary>
    public void SetCalibrationRequirement(bool required)
    {
        RequiresCalibration = required;

        if (required && !LastCalibrationDate.HasValue)
            NextMaintenanceDate = DateTime.UtcNow;
    }

    /// <summary>
    /// Konum bilgisini güncelle
    /// </summary>
    public void UpdateLocation(string location)
    {
        if (string.IsNullOrWhiteSpace(location))
            throw new DomainException("Konum boş olamaz.");

        Location = location;
    }

    // ========== Query Methods ==========

    /// <summary>
    /// Ekipman müsait mi kontrol et
    /// </summary>
    public bool IsAvailable()
    {
        return Status == EquipmentStatus.Operational;  // ✅ Available → Operational
    }

    /// <summary>
    /// Ekipman bakım gerekli mi kontrol et
    /// </summary>
    public bool NeedsMaintenance()
    {
        return NextMaintenanceDate.HasValue &&
               DateTime.UtcNow >= NextMaintenanceDate.Value;
    }

    /// <summary>
    /// Ekipman garanti altında mı kontrol et
    /// </summary>
    public bool IsUnderWarranty()
    {
        return WarrantyExpiryDate.HasValue &&
               DateTime.UtcNow < WarrantyExpiryDate.Value;
    }

    /// <summary>
    /// Ekipman kalibrasyon gerekli mi kontrol et
    /// </summary>
    public bool NeedsCalibration()
    {
        if (!RequiresCalibration)
            return false;

        if (!LastCalibrationDate.HasValue)
            return true;

        return DateTime.UtcNow >= LastCalibrationDate.Value.AddMonths(12);
    }

    /// <summary>
    /// Ekipmanın yaşını ay cinsinden döndür
    /// </summary>
    public int GetAgeInMonths()
    {
        if (!PurchaseDate.HasValue)
            return 0;

        return (int)((DateTime.UtcNow - PurchaseDate.Value).TotalDays / 30.44);
    }

    /// <summary>
    /// Ekipmanın halen çalışma durumunda olup olmadığını kontrol et
    /// </summary>
    public bool IsOperational()
    {
        return Status == EquipmentStatus.Operational ||
               Status == EquipmentStatus.Reserved;
    }

    /// <summary>
    /// Garanti süresi dolmaya ne kadar kaldığını gün cinsinden döndür
    /// </summary>
    public int? GetDaysUntilWarrantyExpiry()
    {
        if (!WarrantyExpiryDate.HasValue)
            return null;

        var daysLeft = (int)(WarrantyExpiryDate.Value - DateTime.UtcNow).TotalDays;
        return daysLeft > 0 ? daysLeft : 0;
    }
}
