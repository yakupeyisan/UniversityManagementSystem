using UniversityMS.Domain.Enums;
using UniversityMS.Domain.Interfaces;

namespace UniversityMS.Domain.Events.FacilityEvents;

public class BuildingCreatedEvent : BaseDomainEvent
{
    public Guid BuildingId { get; }
    public string Code { get; }
    public string Name { get; }
    public BuildingType Type { get; }

    public BuildingCreatedEvent(Guid buildingId, string code, string name, BuildingType type)
    {
        BuildingId = buildingId;
        Code = code;
        Name = name;
        Type = type;
    }
}