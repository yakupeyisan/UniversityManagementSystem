using UniversityMS.Domain.Entities.PersonAggregate;
using UniversityMS.Domain.Enums;

namespace UniversityMS.Domain.Specifications;

public class StudentsByDepartmentSpec : BaseSpecification<Student>
{
    public StudentsByDepartmentSpec(Guid departmentId, StudentStatus? status = null)
        : base(s => s.DepartmentId == departmentId &&
                    (!status.HasValue || s.Status == status.Value))
    {
        ApplyOrderBy(s => s.StudentNumber);
    }
}