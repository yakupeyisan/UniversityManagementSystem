using UniversityMS.Domain.Interfaces;

namespace UniversityMS.Domain.Events.FacilityEvents;

public class EquipmentUpdatedEvent : BaseDomainEvent
{
    public Guid EquipmentId { get; }
    public string? Manufacturer { get; }
    public string? Model { get; }
    public string? SerialNumber { get; }

    public EquipmentUpdatedEvent(Guid equipmentId, string? manufacturer, string? model, string? serialNumber)
    {
        EquipmentId = equipmentId;
        Manufacturer = manufacturer;
        Model = model;
        SerialNumber = serialNumber;
    }
}