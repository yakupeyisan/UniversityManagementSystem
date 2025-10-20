using UniversityMS.Domain.Entities.AcademicAggregate;
using UniversityMS.Domain.Filters;

namespace UniversityMS.Domain.Specifications;

public class DepartmentFilteredSpecification : FilteredSpecification<Department>
{
    public DepartmentFilteredSpecification(
        string? filterString,
        int pageNumber,
        int pageSize,
        IFilterParser<Department> filterParser)
        : base(filterString, filterParser, pageNumber, pageSize)
    {
        AddInclude(d => d.Faculty);
        AddOrderBy(d => d.Name);
    }
}