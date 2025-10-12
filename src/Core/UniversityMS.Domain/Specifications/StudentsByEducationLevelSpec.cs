using UniversityMS.Domain.Entities.PersonAggregate;
using UniversityMS.Domain.Enums;

namespace UniversityMS.Domain.Specifications;

public class StudentsByEducationLevelSpec : BaseSpecification<Student>
{
    public StudentsByEducationLevelSpec(EducationLevel educationLevel)
        : base(s => s.EducationLevel == educationLevel && s.Status == StudentStatus.Active)
    {
        ApplyOrderBy(s => s.CurrentSemester);
    }
}