using UniversityMS.Domain.Entities.FacilityAggregate;
using UniversityMS.Domain.Filters;

namespace UniversityMS.Domain.Specifications;

public class ClassroomFilteredSpecification : FilteredSpecification<Classroom>
{
    public ClassroomFilteredSpecification(
        string? filterString,
        int pageNumber,
        int pageSize,
        IFilterParser<Classroom> filterParser)
        : base(filterString, filterParser, pageNumber, pageSize)
    {
        AddOrderBy(c => c.Building);

        AddThenBy(c => c.Floor);
        AddThenBy(c => c.Code);
    }
}