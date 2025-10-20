using UniversityMS.Domain.Entities.PersonAggregate;

namespace UniversityMS.Domain.Specifications;

/// <summary>
/// Personel araması için Specification
/// SearchTerm, DepartmentId ve Position ile filtreleme yapabilir
/// </summary>
public class SearchStaffSpecification : BaseSpecification<Staff>
{
    public SearchStaffSpecification(
        string? searchTerm,
        Guid? departmentId,
        string? position,
        int pageNumber,
        int pageSize)
        : base(s =>
            (string.IsNullOrEmpty(searchTerm) ||
             s.FirstName.Contains(searchTerm) ||
             s.LastName.Contains(searchTerm) ||
             s.Email.Value.Contains(searchTerm)) &&
            (!departmentId.HasValue || s.DepartmentId == departmentId.Value) &&
            (string.IsNullOrEmpty(position) || s.JobTitle == position))
    {
        // ✅ DÜZELTME: Staff entity'sinde Address ve EmergencyContact navigation property'leri var
        AddInclude(s => s.Address!);
        AddInclude(s => s.EmergencyContact!);

        // Ordering
        AddOrderBy(s => s.FirstName);

        // Pagination
        ApplyPaging((pageNumber - 1) * pageSize, pageSize);
    }
}