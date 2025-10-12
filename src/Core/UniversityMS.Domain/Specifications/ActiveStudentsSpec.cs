using UniversityMS.Domain.Entities.PersonAggregate;
using UniversityMS.Domain.Enums;

namespace UniversityMS.Domain.Specifications;

public class ActiveStudentsSpec : BaseSpecification<Student>
{
    public ActiveStudentsSpec()
        : base(s => s.Status == StudentStatus.Active && !s.IsDeleted)
    {
        ApplyOrderBy(s => s.StudentNumber);
    }
}