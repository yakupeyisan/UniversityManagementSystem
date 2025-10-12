using UniversityMS.Domain.Entities.PersonAggregate;

namespace UniversityMS.Domain.Specifications;

public class StaffByEmployeeNumberSpec : BaseSpecification<Staff>
{
    public StaffByEmployeeNumberSpec(string employeeNumber)
        : base(s => s.EmployeeNumber == employeeNumber)
    {
        AddInclude(s => s.Address!);
        AddInclude(s => s.EmergencyContact!);
    }
}