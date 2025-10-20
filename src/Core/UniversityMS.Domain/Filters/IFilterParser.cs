using System.Linq.Expressions;

namespace UniversityMS.Domain.Filters;

/// <summary>
/// Filter string'ini Expression'a dönüştüren interface
/// Örnek: "price|gt|100;name|contains|book" → Expression<Func<T, bool>>
/// </summary>
public interface IFilterParser<T> where T : class
{
    /// <summary>
    /// Filter string'ini parse edip Expression oluştur
    /// </summary>
    Expression<Func<T, bool>> Parse(string? filterString);

    /// <summary>
    /// Tek bir FilterExpression'dan predicate oluştur
    /// </summary>
    Expression<Func<T, bool>> BuildPredicate(FilterExpression filter);
}