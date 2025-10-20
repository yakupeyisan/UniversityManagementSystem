using UniversityMS.Domain.Entities.HRAggregate;

namespace UniversityMS.Domain.Specifications;

public class EmployeesByDepartmentSpecification : BaseSpecification<Employee>
{
    public EmployeesByDepartmentSpecification(Guid departmentId)
        : base(e => e.DepartmentId == departmentId && !e.Person.IsDeleted)
    {
        // Navigation properties yükle
        AddInclude(e => e.Person);
        AddInclude(e => e.Department);

        // Sorting
        AddOrderBy(e => e.Person.LastName);
    }
}