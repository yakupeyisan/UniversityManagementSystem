using System.Net;
using UniversityMS.Domain.Entities.Common;
using UniversityMS.Domain.Enums;
using UniversityMS.Domain.Exceptions;
using UniversityMS.Domain.ValueObjects;

namespace UniversityMS.Domain.Entities.IdentityAggregate;

public class User : AuditableEntity, ISoftDelete
{
    public string Username { get; private set; }
    public Email Email { get; private set; }
    public string PasswordHash { get; private set; }
    public string? FirstName { get; private set; }
    public string? LastName { get; private set; }
    public bool IsActive { get; private set; }
    public bool EmailConfirmed { get; private set; }
    public DateTime? LastLoginAt { get; private set; }
    public string? RefreshToken { get; private set; }
    public DateTime? RefreshTokenExpiryTime { get; private set; }

    // Soft Delete
    public bool IsDeleted { get; set; }
    public DateTime? DeletedAt { get; set; }
    public string? DeletedBy { get; set; }

    // Navigation Properties
    private readonly List<UserRole> _userRoles = new();
    public IReadOnlyCollection<UserRole> UserRoles => _userRoles.AsReadOnly();

    private User() { } // EF Core için

    private User(string username, Email email, string passwordHash, string? firstName, string? lastName)
        : base()
    {
        Username = username ?? throw new ArgumentNullException(nameof(username));
        Email = email ?? throw new ArgumentNullException(nameof(email));
        PasswordHash = passwordHash ?? throw new ArgumentNullException(nameof(passwordHash));
        FirstName = firstName;
        LastName = lastName;
        IsActive = true;
        EmailConfirmed = false;
    }

    public static User Create(string username, Email email, string passwordHash,
        string? firstName = null, string? lastName = null)
    {
        if (string.IsNullOrWhiteSpace(username))
            throw new DomainException("Kullanıcı adı boş olamaz.");

        if (username.Length < 3)
            throw new DomainException("Kullanıcı adı en az 3 karakter olmalıdır.");

        return new User(username.Trim(), email, passwordHash, firstName?.Trim(), lastName?.Trim());
    }

    public void UpdateProfile(string? firstName, string? lastName)
    {
        FirstName = firstName?.Trim();
        LastName = lastName?.Trim();
    }

    public void ConfirmEmail()
    {
        EmailConfirmed = true;
    }

    public void UpdatePassword(string newPasswordHash)
    {
        if (string.IsNullOrWhiteSpace(newPasswordHash))
            throw new DomainException("Şifre hash değeri boş olamaz.");

        PasswordHash = newPasswordHash;
    }

    public void Activate()
    {
        IsActive = true;
    }

    public void Deactivate()
    {
        IsActive = false;
    }

    public void UpdateLastLogin()
    {
        LastLoginAt = DateTime.UtcNow;
    }

    public void SetRefreshToken(string refreshToken, DateTime expiryTime)
    {
        RefreshToken = refreshToken;
        RefreshTokenExpiryTime = expiryTime;
    }

    public void ClearRefreshToken()
    {
        RefreshToken = null;
        RefreshTokenExpiryTime = null;
    }

    public void AddRole(Role role)
    {
        if (_userRoles.Any(ur => ur.RoleId == role.Id))
            throw new DomainException($"Kullanıcı zaten '{role.Name}' rolüne sahip.");

        _userRoles.Add(UserRole.Create(Id, role.Id));
    }

    public void RemoveRole(Guid roleId)
    {
        var userRole = _userRoles.FirstOrDefault(ur => ur.RoleId == roleId);
        if (userRole != null)
            _userRoles.Remove(userRole);
    }

    public void Delete(string? deletedBy)
    {
        IsDeleted = true;
        DeletedAt = DateTime.UtcNow;
        IsActive = false;
        DeletedBy = deletedBy;
    }

    public void Restore()
    {
        IsDeleted = false;
        DeletedAt = null;
        DeletedBy = null;
        IsActive = true;
    }
}

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

public class UserRole : BaseEntity
{
    public Guid UserId { get; private set; }
    public User User { get; private set; } = null!;

    public Guid RoleId { get; private set; }
    public Role Role { get; private set; } = null!;

    private UserRole() { } // EF Core için

    private UserRole(Guid userId, Guid roleId)
        : base()
    {
        UserId = userId;
        RoleId = roleId;
    }

    public static UserRole Create(Guid userId, Guid roleId)
    {
        return new UserRole(userId, roleId);
    }
}

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
