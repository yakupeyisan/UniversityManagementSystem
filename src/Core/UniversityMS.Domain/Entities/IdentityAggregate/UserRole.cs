using UniversityMS.Domain.Entities.Common;

namespace UniversityMS.Domain.Entities.IdentityAggregate;

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