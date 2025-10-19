using UniversityMS.Domain.Entities.HRAggregate;

namespace UniversityMS.Domain.Specifications;
public class EmployeeSearchSpecification : BaseSpecification<Employee>
{
    public EmployeeSearchSpecification(
        string? searchTerm,
        Guid? departmentId,
        int pageNumber,
        int pageSize)
        : base(e =>
            (string.IsNullOrEmpty(searchTerm) ||
             e.EmployeeNumber.Value.Contains(searchTerm) ||
             e.JobTitle.Contains(searchTerm)) &&
            (!departmentId.HasValue || e.DepartmentId == departmentId))
    {
        AddInclude(e => e.Department);
        AddInclude(e => e.Person);

        // Doğru kullanım - BaseSpecification'de zaten tanımlı
        ApplyPaging(pageNumber, pageSize);
    }
}