using UniversityMS.Domain.Interfaces;

namespace UniversityMS.Domain.Events.InventoryEvents;

public class WarehouseCreatedEvent : BaseDomainEvent
{
    public Guid WarehouseId { get; }
    public string Code { get; }
    public string Name { get; }
    public Guid CampusId { get; }

    public WarehouseCreatedEvent(Guid warehouseId, string code, string name, Guid campusId)
    {
        WarehouseId = warehouseId;
        Code = code;
        Name = name;
        CampusId = campusId;
    }
}