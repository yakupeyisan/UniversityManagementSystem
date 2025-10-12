using UniversityMS.Domain.Entities.PersonAggregate;

namespace UniversityMS.Domain.Specifications;

public class StudentByNumberSpec : BaseSpecification<Student>
{
    public StudentByNumberSpec(string studentNumber)
        : base(s => s.StudentNumber == studentNumber)
    {
        AddInclude(s => s.Address!);
        AddInclude(s => s.EmergencyContact!);
    }
}