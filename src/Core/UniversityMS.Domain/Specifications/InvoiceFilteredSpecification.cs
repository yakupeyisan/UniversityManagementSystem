using System.Linq.Expressions;
using UniversityMS.Domain.Entities.FinanceAggregate;
using UniversityMS.Domain.Interfaces;

namespace UniversityMS.Domain.Specifications;

public class InvoiceFilteredSpecification : ISpecification<Invoice>
{
    public Expression<Func<Invoice, bool>>? Criteria { get; }
    public List<Expression<Func<Invoice, object>>> Includes { get; } = new();
    public List<string> IncludeStrings { get; } = new();
    public Expression<Func<Invoice, object>>? OrderBy { get; }
    public Expression<Func<Invoice, object>>? OrderByDescending { get; }
    public List<Expression<Func<Invoice, object>>> OrderByDescriptors { get; } = new();
    public List<bool> IsOrderByDescending { get; } = new();
    public int Take { get; private set; }
    public int Skip { get; private set; }
    public bool IsPagingEnabled { get; private set; }

    public InvoiceFilteredSpecification(
        string? status = null,
        string? type = null,
        DateTime? fromDate = null,
        DateTime? toDate = null,
        Guid? supplierId = null,
        int pageNumber = 1,
        int pageSize = 10)
    {
        Expression<Func<Invoice, bool>> criteria = i =>
            (string.IsNullOrEmpty(status) || i.Status.ToString() == status) &&
            (string.IsNullOrEmpty(type) || i.Type.ToString() == type) &&
            (!fromDate.HasValue || i.InvoiceDate >= fromDate.Value) &&
            (!toDate.HasValue || i.InvoiceDate <= toDate.Value) &&
            (!supplierId.HasValue || i.SupplierId == supplierId.Value);

        Criteria = criteria;
        Includes.Add(i => i.Items);
        OrderByDescending = i => i.InvoiceDate;

        Skip = (pageNumber - 1) * pageSize;
        Take = pageSize;
        IsPagingEnabled = true;
    }
}