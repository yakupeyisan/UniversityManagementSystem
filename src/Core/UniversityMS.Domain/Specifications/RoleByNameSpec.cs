using UniversityMS.Domain.Entities.IdentityAggregate;

namespace UniversityMS.Domain.Specifications;

public class RoleByNameSpec : BaseSpecification<Role>
{
    public RoleByNameSpec(string roleName)
        : base(r => r.Name == roleName)
    {
    }
}