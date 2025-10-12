using UniversityMS.Domain.Entities.PersonAggregate;

namespace UniversityMS.Domain.Specifications;

public class StaffByDepartmentSpec : BaseSpecification<Staff>
{
    public StaffByDepartmentSpec(Guid departmentId)
        : base(s => s.DepartmentId == departmentId && s.IsActive)
    {
        ApplyOrderBy(s => s.EmployeeNumber);
    }
}