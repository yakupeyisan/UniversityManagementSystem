using UniversityMS.Domain.Entities.PayrollAggregate;

namespace UniversityMS.Domain.Specifications;

/// <summary>
/// Bordro Filtreleme ve Pagination Specification
/// </summary>
public class PayrollFilteredSpecification : BaseSpecification<Payroll>
{
    /// <summary>
    /// Bordroları filtreleme ve sayfalama ile getir
    /// </summary>
    public PayrollFilteredSpecification(
        int pageNumber = 1,
        int pageSize = 50,
        string? status = null,
        int? year = null,
        int? month = null,
        Guid? employeeId = null,
        bool orderByDescending = true)
        : base(p =>
            (string.IsNullOrEmpty(status) || p.Status.ToString() == status) &&
            (!year.HasValue || p.Year == year.Value) &&
            (!month.HasValue || p.Month == month.Value) &&
            (!employeeId.HasValue || p.EmployeeId == employeeId.Value))
    {
        // ========== INCLUDE ==========
        AddInclude(p => p.Employee);
        AddInclude(p => p.Employee.Person);
        AddInclude(p => p.Employee.Department);

        // ========== ORDERING ==========
        if (orderByDescending)
        {
            AddOrderByDescending(p => p.Year);
            AddThenByDescending(p => p.Month);
            AddThenByDescending(p => p.CreatedAt);
        }
        else
        {
            AddOrderBy(p => p.Year);
            AddThenBy(p => p.Month);
            AddThenBy(p => p.CreatedAt);
        }

        // ========== PAGINATION ==========
        ApplyPaging(pageNumber, pageSize);
    }

    /// <summary>
    /// Belirli bir döneme ait bordroları getir (rapor için)
    /// </summary>
    public PayrollFilteredSpecification(
        int month,
        int year,
        int pageNumber = 1,
        int pageSize = 50)
        : base(p => p.Month == month && p.Year == year)
    {
        AddInclude(p => p.Employee);
        AddInclude(p => p.Employee.Person);
        AddOrderBy(p => p.Employee.Person.LastName);
        AddThenBy(p => p.Employee.Person.FirstName);
        ApplyPaging(pageNumber, pageSize);
    }

    /// <summary>
    /// Onay bekleyen bordroları getir
    /// </summary>
    public PayrollFilteredSpecification(
        string status,
        int pageNumber = 1,
        int pageSize = 50)
        : base(p => p.Status.ToString() == status)
    {
        AddInclude(p => p.Employee);
        AddInclude(p => p.Employee.Person);
        AddOrderByDescending(p => p.CreatedAt);
        ApplyPaging(pageNumber, pageSize);
    }
}