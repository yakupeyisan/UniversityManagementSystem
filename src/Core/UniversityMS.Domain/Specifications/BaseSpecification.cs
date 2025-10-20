using System.Linq.Expressions;
using UniversityMS.Domain.Entities.FacilityAggregate;
using UniversityMS.Domain.Interfaces;

namespace UniversityMS.Domain.Specifications;


public abstract class BaseSpecification<T> : ISpecification<T>
{
    public Expression<Func<T, bool>>? Criteria { get; private set; }
    public List<Expression<Func<T, object>>> Includes { get; } = new();
    public List<string> IncludeStrings { get; } = new();
    public Expression<Func<T, object>>? OrderBy { get; private set; }
    public Expression<Func<T, object>>? OrderByDescending { get; private set; }
    public List<Expression<Func<T, object>>> OrderByDescriptors { get; } = new();
    public List<bool> IsOrderByDescending { get; } = new();
    public int Take { get; private set; }
    public int Skip { get; private set; }
    public bool IsPagingEnabled { get; private set; }

    protected BaseSpecification()
    {
    }

    protected BaseSpecification(Expression<Func<T, bool>> criteria)
    {
        Criteria = criteria;
    }

    protected virtual void AddInclude(Expression<Func<T, object>> includeExpression)
    {
        Includes.Add(includeExpression);
    }

    protected virtual void AddInclude(string includeString)
    {
        IncludeStrings.Add(includeString);
    }

    protected virtual void ApplyPaging(int skip, int take)
    {
        Skip = skip;
        Take = take;
        IsPagingEnabled = true;
    }

    protected virtual void ApplyOrderBy(Expression<Func<T, object>> orderByExpression)
    {
        OrderBy = orderByExpression;
    }

    protected virtual void ApplyOrderByDescending(Expression<Func<T, object>> orderByDescExpression)
    {
        OrderByDescending = orderByDescExpression;
    }
    protected virtual void AddOrderBy(Expression<Func<T, object>> orderByExpression)
    {
        OrderBy = orderByExpression;
    }

    protected virtual void AddOrderByDescending(Expression<Func<T, object>> orderByDescendingExpression)
    {
        OrderByDescending = orderByDescendingExpression;
    }
    protected virtual void AddThenBy(Expression<Func<T, object>> thenByExpression)
    {
        OrderByDescriptors.Add(thenByExpression);
        IsOrderByDescending.Add(false);
    }

    protected virtual void AddThenByDescending(Expression<Func<T, object>> thenByDescendingExpression)
    {
        OrderByDescriptors.Add(thenByDescendingExpression);
        IsOrderByDescending.Add(true);
    }

}