using System.Linq.Expressions;
using UniversityMS.Domain.Entities.ProcurementAggregate;
using UniversityMS.Domain.Interfaces;

namespace UniversityMS.Domain.Specifications;

public class PurchaseRequestFilteredSpecification : ISpecification<PurchaseRequest>
{
    public Expression<Func<PurchaseRequest, bool>>? Criteria { get; }
    public List<Expression<Func<PurchaseRequest, object>>> Includes { get; } = new();
    public List<string> IncludeStrings { get; } = new();
    public Expression<Func<PurchaseRequest, object>>? OrderBy { get; }
    public Expression<Func<PurchaseRequest, object>>? OrderByDescending { get; }
    public List<Expression<Func<PurchaseRequest, object>>> OrderByDescriptors { get; } = new();
    public List<bool> IsOrderByDescending { get; } = new();
    public int Take { get; private set; }
    public int Skip { get; private set; }
    public bool IsPagingEnabled { get; private set; }

    public PurchaseRequestFilteredSpecification(string? status = null, Guid? departmentId = null,
        int pageNumber = 1, int pageSize = 10)
    {
        Expression<Func<PurchaseRequest, bool>> criteria = pr =>
            !pr.IsDeleted &&
            (string.IsNullOrEmpty(status) || pr.Status.ToString() == status) &&
            (!departmentId.HasValue || pr.DepartmentId == departmentId.Value);

        Criteria = criteria;
        Includes.Add(pr => pr.Items);
        OrderByDescending = pr => pr.CreatedAt;

        Skip = (pageNumber - 1) * pageSize;
        Take = pageSize;
        IsPagingEnabled = true;
    }
}