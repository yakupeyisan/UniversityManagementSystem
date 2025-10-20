using System.Linq.Expressions;
using UniversityMS.Domain.Entities.FinanceAggregate;
using UniversityMS.Domain.Interfaces;

namespace UniversityMS.Domain.Specifications;

public class BudgetFilteredSpecification : ISpecification<Budget>
{
    public Expression<Func<Budget, bool>>? Criteria { get; }
    public List<Expression<Func<Budget, object>>> Includes { get; } = new();
    public List<string> IncludeStrings { get; } = new();
    public Expression<Func<Budget, object>>? OrderBy { get; }
    public Expression<Func<Budget, object>>? OrderByDescending { get; }
    public List<Expression<Func<Budget, object>>> OrderByDescriptors { get; } = new();
    public List<bool> IsOrderByDescending { get; } = new();
    public int Take { get; private set; }
    public int Skip { get; private set; }
    public bool IsPagingEnabled { get; private set; }

    public BudgetFilteredSpecification(
        int? fiscalYear = null,
        Guid? departmentId = null,
        string? status = null,
        int pageNumber = 1,
        int pageSize = 10)
    {
        Expression<Func<Budget, bool>> criteria = b =>
            (!fiscalYear.HasValue || b.FiscalYear == fiscalYear.Value) &&
            (!departmentId.HasValue || b.DepartmentId == departmentId.Value) &&
            (string.IsNullOrEmpty(status) || b.Status.ToString() == status);

        Criteria = criteria;
        Includes.Add(b => b.Items);
        OrderByDescending = b => b.FiscalYear;

        Skip = (pageNumber - 1) * pageSize;
        Take = pageSize;
        IsPagingEnabled = true;
    }
}