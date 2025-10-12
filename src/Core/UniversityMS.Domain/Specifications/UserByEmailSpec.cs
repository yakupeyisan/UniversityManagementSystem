using UniversityMS.Domain.Entities.IdentityAggregate;

namespace UniversityMS.Domain.Specifications;

public class UserByEmailSpec : BaseSpecification<User>
{
    public UserByEmailSpec(string email)
        : base(u => u.Email.Value == email && !u.IsDeleted)
    {
        AddInclude(u => u.UserRoles);
    }
}