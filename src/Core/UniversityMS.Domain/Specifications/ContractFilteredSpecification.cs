using System.Linq.Expressions;
using UniversityMS.Domain.Entities.HRAggregate;

namespace UniversityMS.Domain.Specifications;

/// <summary>
/// Sözleşmeleri filtreleme ve sayfalama için specification
/// Çalışan, Status ve Sözleşme Türü ile filtreleme yapabilir
/// </summary>
public class ContractFilteredSpecification : FilteredSpecification<Contract>
{
    public ContractFilteredSpecification(
        Guid? employeeId = null,
        string? status = null,
        string? contractType = null,
        int pageNumber = 1,
        int pageSize = 10)
        : base(BuildCriteria(employeeId, status, contractType))
    {
        // Include'ları ekle
        AddInclude(c => c.Employee);

        // Sıralama: Önce başlangıç tarihi, sonra sözleşme numarası
        ApplyOrderByDescending(c => c.StartDate);
        AddThenByDescending(c => c.ContractNumber);

        // Sayfalama
        ApplyPaging((pageNumber - 1) * pageSize, pageSize);
    }

    private static Expression<Func<Contract, bool>> BuildCriteria(
        Guid? employeeId,
        string? status,
        string? contractType)
    {
        return c =>
            (employeeId == null || c.EmployeeId == employeeId) &&
            (string.IsNullOrEmpty(status) || c.Status.ToString() == status) &&
            (string.IsNullOrEmpty(contractType) || c.Type.ToString() == contractType);
    }
}