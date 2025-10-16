using UniversityMS.Domain.Entities.IdentityAggregate;

namespace UniversityMS.Domain.Specifications;

public class UserByNationalIdSpec : BaseSpecification<User>
{
    public UserByNationalIdSpec(string nationalId)
        : base(u => u.Username == nationalId)
    {
    }
}