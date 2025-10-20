using UniversityMS.Domain.Entities.HRAggregate;
using UniversityMS.Domain.Filters;

namespace UniversityMS.Domain.Specifications;
public class EmployeeFilteredSpecification : FilteredSpecification<Employee>
{
    public EmployeeFilteredSpecification(
        string? filterString,
        int pageNumber,
        int pageSize,
        IFilterParser<Employee> filterParser)
        : base(filterString, filterParser, pageNumber, pageSize)
    {
        AddInclude(e => e.Person);
        AddInclude(e => e.Department);
        AddOrderBy(e => e.Person.LastName);
    }
}