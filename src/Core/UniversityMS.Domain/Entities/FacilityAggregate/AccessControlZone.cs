using UniversityMS.Domain.Entities.Common;
using UniversityMS.Domain.Enums;
using UniversityMS.Domain.Exceptions;

namespace UniversityMS.Domain.Entities.FacilityAggregate;

public class AccessControlZone : AuditableEntity
{
    public Guid LaboratoryId { get; private set; }
    public string Name { get; private set; } = string.Empty;
    public AccessControlLevel AccessLevel { get; private set; }
    public bool RequiresSafetyTraining { get; private set; }
    public List<string> AuthorizedRoles { get; private set; } = new();
    public List<string> RequiredCertifications { get; private set; } = new();

    // Navigation
    public Laboratory Laboratory { get; private set; } = null!;

    private AccessControlZone() { }

    private AccessControlZone(Guid laboratoryId, string name, AccessControlLevel level)
    {
        LaboratoryId = laboratoryId;
        Name = name;
        AccessLevel = level;
        RequiresSafetyTraining = level >= AccessControlLevel.High;
    }

    public static AccessControlZone Create(Guid laboratoryId, string name, AccessControlLevel level)
    {
        if (laboratoryId == Guid.Empty)
            throw new DomainException("Laboratuvar ID boş olamaz.");
        if (string.IsNullOrWhiteSpace(name))
            throw new DomainException("Bölge adı boş olamaz.");

        return new AccessControlZone(laboratoryId, name, level);
    }

    public void AddAuthorizedRole(string role) => AuthorizedRoles.Add(role);
    public void AddRequiredCertification(string certification) => RequiredCertifications.Add(certification);
}