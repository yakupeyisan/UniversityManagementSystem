using UniversityMS.Domain.Enums;
using UniversityMS.Domain.Interfaces;

namespace UniversityMS.Domain.Events.FacilityEvents;

public class LaboratoryCreatedEvent : BaseDomainEvent
{
    public Guid LaboratoryId { get; }
    public string Code { get; }
    public LaboratoryType Type { get; }
    public SafetyLevel SafetyLevel { get; }

    public LaboratoryCreatedEvent(Guid laboratoryId, string code, LaboratoryType type, SafetyLevel safetyLevel)
    {
        LaboratoryId = laboratoryId;
        Code = code;
        Type = type;
        SafetyLevel = safetyLevel;
    }
}