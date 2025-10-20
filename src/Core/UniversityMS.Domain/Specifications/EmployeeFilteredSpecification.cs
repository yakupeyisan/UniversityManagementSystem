using System.Linq.Expressions;
using UniversityMS.Domain.Entities.HRAggregate;
using UniversityMS.Domain.Filters;
using UniversityMS.Domain.Interfaces;

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
    public EmployeeFilteredSpecification(
        Guid? departmentId = null,
        string? status = null,
        string? searchTerm = null,
        int pageNumber = 1,
        int pageSize = 10):base(BuildCriteria(departmentId, status, searchTerm))

    {
        
        // Include'ları ekle
        AddInclude(e => e.Person);
        AddInclude(e => e.Department);

        // Sıralama
        ApplyOrderByDescending(e => e.HireDate);

        // Sayfalama
        ApplyPaging((pageNumber - 1) * pageSize, pageSize);
    }

    private static Expression<Func<Employee, bool>> BuildCriteria(
        Guid? departmentId,
        string? status,
        string? searchTerm)
    {
        return e =>
            (departmentId == null || e.DepartmentId == departmentId) &&
            (string.IsNullOrEmpty(status) || e.Status.ToString() == status) &&
            (string.IsNullOrEmpty(searchTerm) ||
             e.JobTitle.Contains(searchTerm) ||
             e.EmployeeNumber.ToString().Contains(searchTerm));
    }
}