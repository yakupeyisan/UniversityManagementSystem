using System.Linq.Expressions;
using UniversityMS.Domain.Entities.HRAggregate;

namespace UniversityMS.Domain.Specifications;

/// <summary>
/// İzinleri filtreleme ve sayfalama için specification
/// Çalışan, Status, İzin Türü ve tarih aralığı ile filtreleme yapabilir
/// </summary>
public class LeaveFilteredSpecification : BaseFilteredSpecification<Leave>
{
    public LeaveFilteredSpecification(
        Guid? employeeId = null,
        string? status = null,
        string? leaveType = null,
        DateTime? fromDate = null,
        DateTime? toDate = null,
        int pageNumber = 1,
        int pageSize = 10)
        : base(BuildCriteria(employeeId, status, leaveType, fromDate, toDate))
    {
        // Include'ları ekle
        AddInclude(l => l.Employee);

        // Sıralama
        ApplyOrderByDescending(l => l.StartDate);

        // Sayfalama
        ApplyPaging((pageNumber - 1) * pageSize, pageSize);
    }

    private static Expression<Func<Leave, bool>> BuildCriteria(
        Guid? employeeId,
        string? status,
        string? leaveType,
        DateTime? fromDate,
        DateTime? toDate)
    {
        return l =>
            (employeeId == null || l.EmployeeId == employeeId) &&
            (string.IsNullOrEmpty(status) || l.Status.ToString() == status) &&
            (string.IsNullOrEmpty(leaveType) || l.LeaveType.ToString() == leaveType) &&
            (fromDate == null || l.StartDate.Date >= fromDate.Value.Date) &&
            (toDate == null || l.EndDate.Date <= toDate.Value.Date);
    }
}