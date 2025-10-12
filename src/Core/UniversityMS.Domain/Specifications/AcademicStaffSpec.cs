using UniversityMS.Domain.Entities.PersonAggregate;

namespace UniversityMS.Domain.Specifications;

public class AcademicStaffSpec : BaseSpecification<Staff>
{
    public AcademicStaffSpec(Guid? departmentId = null)
        : base(s => s.AcademicTitle.HasValue &&
                    s.IsActive &&
                    (!departmentId.HasValue || s.DepartmentId == departmentId))
    {
        ApplyOrderBy(s => s.AcademicTitle);
    }
}