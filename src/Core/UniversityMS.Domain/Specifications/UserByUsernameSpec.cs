using UniversityMS.Domain.Entities.IdentityAggregate;

namespace UniversityMS.Domain.Specifications;

public class UserByUsernameSpec : BaseSpecification<User>
{
    public UserByUsernameSpec(string username)
        : base(u => u.Username == username && !u.IsDeleted)
    {
        AddInclude(u => u.UserRoles);
        AddInclude("UserRoles.Role");
        AddInclude("UserRoles.Role.Permissions");
    }
}