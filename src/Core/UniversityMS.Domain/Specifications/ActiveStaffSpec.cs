using UniversityMS.Domain.Entities.PersonAggregate;

namespace UniversityMS.Domain.Specifications;

public class ActiveStaffSpec : BaseSpecification<Staff>
{
    public ActiveStaffSpec()
        : base(s => s.IsActive && !s.IsDeleted)
    {
        ApplyOrderBy(s => s.EmployeeNumber);
    }
}