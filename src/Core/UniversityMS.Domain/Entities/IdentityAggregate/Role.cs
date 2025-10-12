using UniversityMS.Domain.Entities.Common;
using UniversityMS.Domain.Exceptions;

namespace UniversityMS.Domain.Entities.IdentityAggregate;

public class Role : AuditableEntity
{
    public string Name { get; private set; }
    public string? Description { get; private set; }
    public bool IsSystemRole { get; private set; }

    // Navigation Properties
    private readonly List<UserRole> _userRoles = new();
    public IReadOnlyCollection<UserRole> UserRoles => _userRoles.AsReadOnly();

    private readonly List<Permission> _permissions = new();
    public IReadOnlyCollection<Permission> Permissions => _permissions.AsReadOnly();

    private Role() { } // EF Core için

    private Role(string name, string? description, bool isSystemRole)
        : base()
    {
        Name = name;
        Description = description;
        IsSystemRole = isSystemRole;
    }

    public static Role Create(string name, string? description = null, bool isSystemRole = false)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new DomainException("Rol adı boş olamaz.");

        return new Role(name.Trim(), description?.Trim(), isSystemRole);
    }

    public void Update(string name, string? description)
    {
        if (IsSystemRole)
            throw new DomainException("Sistem rolleri güncellenemez.");

        if (string.IsNullOrWhiteSpace(name))
            throw new DomainException("Rol adı boş olamaz.");

        Name = name.Trim();
        Description = description?.Trim();
    }

    public void AddPermission(Permission permission)
    {
        if (_permissions.Any(p => p.Id == permission.Id))
            return;

        _permissions.Add(permission);
    }

    public void RemovePermission(Permission permission)
    {
        _permissions.Remove(permission);
    }
}