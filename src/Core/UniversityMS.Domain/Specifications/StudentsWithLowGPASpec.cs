using UniversityMS.Domain.Entities.PersonAggregate;
using UniversityMS.Domain.Enums;

namespace UniversityMS.Domain.Specifications;

public class StudentsWithLowGPASpec : BaseSpecification<Student>
{
    public StudentsWithLowGPASpec(double cgpaThreshold = 2.0)
        : base(s => s.CGPA < cgpaThreshold && s.Status == StudentStatus.Active)
    {
        ApplyOrderBy(s => s.CGPA);
    }
}