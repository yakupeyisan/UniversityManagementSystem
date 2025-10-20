using System.Linq.Expressions;
using UniversityMS.Domain.Entities.ProcurementAggregate;
using UniversityMS.Domain.Interfaces;

namespace UniversityMS.Domain.Specifications;

public class PurchaseOrderFilteredSpecification : ISpecification<PurchaseOrder>
{
    public Expression<Func<PurchaseOrder, bool>>? Criteria { get; }
    public List<Expression<Func<PurchaseOrder, object>>> Includes { get; } = new();
    public List<string> IncludeStrings { get; } = new();
    public Expression<Func<PurchaseOrder, object>>? OrderBy { get; }
    public Expression<Func<PurchaseOrder, object>>? OrderByDescending { get; }
    public List<Expression<Func<PurchaseOrder, object>>> OrderByDescriptors { get; } = new();
    public List<bool> IsOrderByDescending { get; } = new();
    public int Take { get; private set; }
    public int Skip { get; private set; }
    public bool IsPagingEnabled { get; private set; }

    public PurchaseOrderFilteredSpecification(
        string? status = null,
        Guid? supplierId = null,
        DateTime? fromDate = null,
        DateTime? toDate = null,
        int pageNumber = 1,
        int pageSize = 10)
    {
        Expression<Func<PurchaseOrder, bool>> criteria = po =>
            (string.IsNullOrEmpty(status) || po.Status.ToString() == status) &&
            (!supplierId.HasValue || po.SupplierId == supplierId.Value) &&
            (!fromDate.HasValue || po.OrderDate >= fromDate.Value) &&
            (!toDate.HasValue || po.OrderDate <= toDate.Value);

        Criteria = criteria;
        Includes.Add(po => po.Items);
        OrderByDescending = po => po.OrderDate;

        Skip = (pageNumber - 1) * pageSize;
        Take = pageSize;
        IsPagingEnabled = true;
    }
}