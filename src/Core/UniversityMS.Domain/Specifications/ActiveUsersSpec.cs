using UniversityMS.Domain.Entities.IdentityAggregate;

namespace UniversityMS.Domain.Specifications;

public class ActiveUsersSpec : BaseSpecification<User>
{
    public ActiveUsersSpec()
        : base(u => u.IsActive && !u.IsDeleted)
    {
        ApplyOrderBy(u => u.Username);
    }
}