using System.Linq.Expressions;
using UniversityMS.Domain.Entities.ProcurementAggregate;
using UniversityMS.Domain.Interfaces;

namespace UniversityMS.Domain.Specifications;

public class SupplierFilteredSpecification : ISpecification<Supplier>
{
    public Expression<Func<Supplier, bool>>? Criteria { get; }
    public List<Expression<Func<Supplier, object>>> Includes { get; } = new();
    public List<string> IncludeStrings { get; } = new();
    public Expression<Func<Supplier, object>>? OrderBy { get; }
    public Expression<Func<Supplier, object>>? OrderByDescending { get; }
    public List<Expression<Func<Supplier, object>>> OrderByDescriptors { get; } = new();
    public List<bool> IsOrderByDescending { get; } = new();
    public int Take { get; private set; }
    public int Skip { get; private set; }
    public bool IsPagingEnabled { get; private set; }

    public SupplierFilteredSpecification(
        string? status = null,
        string? searchTerm = null,
        int pageNumber = 1,
        int pageSize = 10)
    {
        Expression<Func<Supplier, bool>> criteria = s =>
            (string.IsNullOrEmpty(status) || s.Status.ToString() == status) &&
            (string.IsNullOrEmpty(searchTerm) ||
             s.Name.Contains(searchTerm) ||
             s.Code.Contains(searchTerm));

        Criteria = criteria;
        OrderBy = s => s.Name;

        Skip = (pageNumber - 1) * pageSize;
        Take = pageSize;
        IsPagingEnabled = true;
    }
}