using UniversityMS.Domain.Entities.Common;
using UniversityMS.Domain.Exceptions;

namespace UniversityMS.Domain.Entities.IdentityAggregate;

public class Permission : AuditableEntity
{
    public string Name { get; private set; }
    public string Resource { get; private set; }
    public string Action { get; private set; }
    public string? Description { get; private set; }

    private Permission() { } // EF Core için

    private Permission(string name, string resource, string action, string? description)
        : base()
    {
        Name = name;
        Resource = resource;
        Action = action;
        Description = description;
    }

    public static Permission Create(string name, string resource, string action, string? description = null)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new DomainException("İzin adı boş olamaz.");

        if (string.IsNullOrWhiteSpace(resource))
            throw new DomainException("Kaynak adı boş olamaz.");

        if (string.IsNullOrWhiteSpace(action))
            throw new DomainException("Aksiyon boş olamaz.");

        return new Permission(name.Trim(), resource.Trim(), action.Trim(), description?.Trim());
    }
}