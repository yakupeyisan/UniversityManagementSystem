using UniversityMS.Domain.Entities.EnrollmentAggregate;
using UniversityMS.Domain.Filters;

namespace UniversityMS.Domain.Specifications;

public class EnrollmentFilteredSpecification : FilteredSpecification<Enrollment>
{
    public EnrollmentFilteredSpecification(
        string? filterString,
        int pageNumber,
        int pageSize,
        IFilterParser<Enrollment> filterParser)
        : base(filterString, filterParser, pageNumber, pageSize)
    {
        AddInclude(e => e.Student);
        AddInclude(e => e.CourseRegistrations);
        AddOrderByDescending(e => e.AcademicYear);
        AddOrderByDescending(e => e.Semester);
    }
}