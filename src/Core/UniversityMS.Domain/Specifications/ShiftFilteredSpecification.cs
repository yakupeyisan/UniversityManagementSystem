using System.Linq.Expressions;
using UniversityMS.Domain.Entities.HRAggregate;

namespace UniversityMS.Domain.Specifications;

/// <summary>
/// Vardiyaları filtreleme ve sayfalama için specification
/// Çalışan, Status ve tarih aralığı ile filtreleme yapabilir
/// </summary>
public class ShiftFilteredSpecification : BaseFilteredSpecification<Shift>
{
    public ShiftFilteredSpecification(
        Guid? employeeId = null,
        string? status = null,
        DateTime? fromDate = null,
        DateTime? toDate = null,
        int pageNumber = 1,
        int pageSize = 10)
        : base(BuildCriteria(employeeId, status, fromDate, toDate))
    {
        // Include'ları ekle
        AddInclude(s => s.Employee);

        // Sıralama
        ApplyOrderByDescending(s => s.Date);

        // Sayfalama
        ApplyPaging((pageNumber - 1) * pageSize, pageSize);
    }

    private static Expression<Func<Shift, bool>> BuildCriteria(
        Guid? employeeId,
        string? status,
        DateTime? fromDate,
        DateTime? toDate)
    {
        var fromDateOnly = fromDate.HasValue ? DateOnly.FromDateTime(fromDate.Value) : (DateOnly?)null;
        var toDateOnly = toDate.HasValue ? DateOnly.FromDateTime(toDate.Value) : (DateOnly?)null;

        return s =>
            (employeeId == null || s.EmployeeId == employeeId) &&
            (string.IsNullOrEmpty(status) || s.Status.ToString() == status) &&
            (fromDateOnly == null || s.Date >= fromDateOnly.Value) &&
            (toDateOnly == null || s.Date <= toDateOnly.Value);
    }
}