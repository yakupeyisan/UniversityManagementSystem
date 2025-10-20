using UniversityMS.Domain.Entities.PayrollAggregate;

namespace UniversityMS.Domain.Specifications;

/// <summary>
/// Payslip Filtreleme ve Pagination Specification
/// </summary>
public class PayslipFilteredSpecification : BaseSpecification<Payslip>
{
    /// <summary>
    /// Payslipleri filtreleme ve sayfalama ile getir
    /// </summary>
    public PayslipFilteredSpecification(
        int pageNumber = 1,
        int pageSize = 50,
        Guid? employeeId = null,
        int? year = null,
        int? month = null,
        string? status = null,
        bool orderByDescending = true)
        : base(p =>
            (!employeeId.HasValue || p.EmployeeId == employeeId.Value) &&
            (!year.HasValue || p.Year == year.Value) &&
            (!month.HasValue || p.Month == month.Value) &&
            (string.IsNullOrEmpty(status) || p.Status.ToString() == status))
    {
        // ========== INCLUDE ==========
        AddInclude(p => p.Payroll);

        // ========== ORDERING ==========
        if (orderByDescending)
        {
            AddOrderByDescending(p => p.Year);
            AddThenByDescending(p => p.Month);
            AddThenByDescending(p => p.GeneratedDate);
        }
        else
        {
            AddOrderBy(p => p.Year);
            AddThenBy(p => p.Month);
            AddThenBy(p => p.GeneratedDate);
        }

        // ========== PAGINATION ==========
        ApplyPaging(pageNumber, pageSize);
    }

    /// <summary>
    /// Belirli bir çalışanın paysliplerini getir
    /// </summary>
    public PayslipFilteredSpecification(
        Guid employeeId,
        int pageNumber = 1,
        int pageSize = 50)
        : base(p => p.EmployeeId == employeeId)
    {
        AddInclude(p => p.Payroll);
        AddOrderByDescending(p => p.Year);
        AddThenByDescending(p => p.Month);
        ApplyPaging(pageNumber, pageSize);
    }

    /// <summary>
    /// Belirli döneme ait tüm payslipleri getir
    /// </summary>
    public PayslipFilteredSpecification(
        int month,
        int year,
        int pageNumber = 1,
        int pageSize = 50)
        : base(p => p.Month == month && p.Year == year)
    {
        AddInclude(p => p.Payroll);
        AddOrderBy(p => p.EmployeeId);
        ApplyPaging(pageNumber, pageSize);
    }

    /// <summary>
    /// Email gönderilmesi gereken payslipleri getir
    /// </summary>
    public PayslipFilteredSpecification(
        DateTime fromDate,
        DateTime toDate,
        string status = "Generated",
        int pageNumber = 1,
        int pageSize = 50)
        : base(p =>
            p.GeneratedDate >= fromDate &&
            p.GeneratedDate <= toDate &&
            p.Status.ToString() == status &&
            p.EmailSentDate == null) // Henüz gönderilmemiş
    {
        AddInclude(p => p.Payroll);
        AddOrderBy(p => p.GeneratedDate);
        ApplyPaging(pageNumber, pageSize);
    }
}