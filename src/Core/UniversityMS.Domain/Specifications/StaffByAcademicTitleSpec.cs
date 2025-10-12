using UniversityMS.Domain.Entities.PersonAggregate;
using UniversityMS.Domain.Enums;

namespace UniversityMS.Domain.Specifications;

public class StaffByAcademicTitleSpec : BaseSpecification<Staff>
{
    public StaffByAcademicTitleSpec(AcademicTitle academicTitle)
        : base(s => s.AcademicTitle == academicTitle && s.IsActive)
    {
        ApplyOrderBy(s => s.HireDate);
    }
}