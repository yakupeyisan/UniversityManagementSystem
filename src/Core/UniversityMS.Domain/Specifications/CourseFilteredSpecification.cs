using UniversityMS.Domain.Entities.AcademicAggregate;
using UniversityMS.Domain.Filters;

namespace UniversityMS.Domain.Specifications;

public class CourseFilteredSpecification : BaseFilteredSpecification<Course>
{
    /// <summary>
    /// Filter string ile Course listesi (soft delete kontrollü)
    /// Format: department|eq|{guid};isActive|eq|true;ects|gte|6;code|contains|COMP
    /// </summary>
    public CourseFilteredSpecification(
        string? filterString,
        int pageNumber,
        int pageSize,
        IFilterParser<Course> filterParser)
        : base(filterString, filterParser, pageNumber, pageSize)
    {
        // Navigation properties
        AddInclude(c => c.Department);

        // Default ordering
        AddOrderBy(c => c.Code);
    }
}