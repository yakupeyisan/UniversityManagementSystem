using UniversityMS.Domain.Entities.PersonAggregate;
using UniversityMS.Domain.Filters;

namespace UniversityMS.Domain.Specifications;

public class StudentFilteredSpecification : BaseFilteredSpecification<Student>
{
    public StudentFilteredSpecification(
        string? filterString,
        int pageNumber,
        int pageSize,
        IFilterParser<Student> filterParser)
        : base(filterString, filterParser, pageNumber, pageSize)
    {
        AddInclude(s => s.Address!);
        AddInclude(s => s.EmergencyContact!);
        AddOrderBy(s => s.StudentNumber);
    }
}