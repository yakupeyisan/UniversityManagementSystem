using UniversityMS.Domain.Entities.HRAggregate;
using UniversityMS.Domain.Enums;

namespace UniversityMS.Domain.Specifications;

public class ActiveEmployeesSpecification : BaseSpecification<Employee>
{
    public ActiveEmployeesSpecification(int pageNumber, int pageSize)
        : base(e => e.Status == EmploymentStatus.Active && !e.Person.IsDeleted)
    {
        // Navigation properties yükle
        AddInclude(e => e.Person);
        AddInclude(e => e.Department);

        // Sorting
        AddOrderBy(e => e.Person.LastName);

        // Pagination
        ApplyPaging(skip: (pageNumber - 1) * pageSize, take: pageSize);
    }
}