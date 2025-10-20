using System.Linq.Expressions;

namespace UniversityMS.Domain.Filters;

/// <summary>
/// FilterExpression'dan Lambda Expression oluşturan builder
/// </summary>
public interface IFilterExpressionBuilder<T> where T : class
{
    Expression<Func<T, bool>> Build(FilterExpression filter);
}