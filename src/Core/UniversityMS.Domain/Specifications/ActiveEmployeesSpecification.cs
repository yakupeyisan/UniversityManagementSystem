using UniversityMS.Domain.Entities.HRAggregate;
using UniversityMS.Domain.Enums;

namespace UniversityMS.Domain.Specifications;

/// <summary>
/// Aktif çalışanları filtreleyen Specification
/// </summary>
public class ActiveEmployeesSpecification : BaseSpecification<Employee>
{
    public ActiveEmployeesSpecification(
        EmploymentStatus status,
        int pageNumber,
        int pageSize)
        : base(e => e.Status == status)
    {
        AddInclude(e => e.Person);
        AddInclude(e => e.Department);
        AddOrderBy(e => e.Person.LastName);

        // Pagination
        ApplyPaging((pageNumber - 1) * pageSize, pageSize);
    }
}