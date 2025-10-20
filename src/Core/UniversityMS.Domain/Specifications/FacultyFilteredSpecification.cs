using UniversityMS.Domain.Entities.AcademicAggregate;
using UniversityMS.Domain.Filters;

namespace UniversityMS.Domain.Specifications;

public class FacultyFilteredSpecification : FilteredSpecification<Faculty>
{
    public FacultyFilteredSpecification(
        string? filterString,
        int pageNumber,
        int pageSize,
        IFilterParser<Faculty> filterParser)
        : base(filterString, filterParser, pageNumber, pageSize)
    {
        AddInclude(f => f.Departments);
        AddOrderBy(f => f.Name);
    }
}