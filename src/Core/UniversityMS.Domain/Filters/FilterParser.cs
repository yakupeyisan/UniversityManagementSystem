using System.Linq.Expressions;

namespace UniversityMS.Domain.Filters;

/// <summary>
/// URL filter format'ını Expression<Func<T, bool>>'e dönüştürür
/// Format: field|operator|value;field2|operator2|value2
/// Örnek: price|gt|100;name|contains|book;status|in|active,pending;createdAt|between|2024-01-01,2024-12-31
/// </summary>
public class FilterParser<T> : IFilterParser<T> where T : class
{
    private const char FilterSeparator = ';';
    private const char ExpressionSeparator = '|';
    private const char ValueSeparator = ',';
    private readonly IFilterExpressionBuilder<T> _expressionBuilder;

    public FilterParser(IFilterExpressionBuilder<T>? expressionBuilder = null)
    {
        _expressionBuilder = expressionBuilder ?? new FilterExpressionBuilder<T>();
    }

    /// <summary>
    /// Filter string'ini parse et ve kombinli Expression oluştur
    /// Tüm filterler AND ile kombinlenmiştir
    /// </summary>
    public Expression<Func<T, bool>> Parse(string? filterString)
    {
        if (string.IsNullOrWhiteSpace(filterString))
        {
            // Boş filter: her şey döner (true)
            return x => true;
        }

        try
        {
            var filters = ParseFilterExpressions(filterString);

            if (filters.Count == 0)
                return x => true;

            // İlk filter ile başla
            var predicate = BuildPredicate(filters[0]);

            // Diğer filtreleri AND ile ekle
            for (int i = 1; i < filters.Count; i++)
            {
                var nextFilter = BuildPredicate(filters[i]);
                predicate = CombinePredicates(predicate, nextFilter, ExpressionType.AndAlso);
            }

            return predicate;
        }
        catch (Exception ex)
        {
            throw new FilterParsingException($"Filter string parse hatası: {filterString}", ex);
        }
    }

    /// <summary>
    /// Tek bir FilterExpression'dan LINQ predicate oluştur
    /// </summary>
    public Expression<Func<T, bool>> BuildPredicate(FilterExpression filter)
    {
        return _expressionBuilder.Build(filter);
    }

    // ============================================================================
    // Private Helpers
    // ============================================================================

    /// <summary>
    /// Filter string'ini FilterExpression listesine parse et
    /// </summary>
    private List<FilterExpression> ParseFilterExpressions(string filterString)
    {
        var result = new List<FilterExpression>();
        var filterParts = filterString.Split(FilterSeparator);

        foreach (var part in filterParts)
        {
            if (string.IsNullOrWhiteSpace(part))
                continue;

            var filter = ParseSingleFilter(part.Trim());
            if (filter != null)
                result.Add(filter);
        }

        return result;
    }

    /// <summary>
    /// Tek bir filter string'i parse et
    /// Örnek: "price|gt|100" → FilterExpression
    /// </summary>
    private FilterExpression? ParseSingleFilter(string filterPart)
    {
        var parts = filterPart.Split(ExpressionSeparator);

        if (parts.Length < 2)
            return null;

        var propertyName = parts[0].Trim();
        var operatorStr = parts[1].Trim().ToLower();

        // Operator parse et
        if (!TryParseOperator(operatorStr, out var op))
            throw new FilterParsingException($"Bilinmeyen operator: {operatorStr}");

        // Null/NotNull operatörleri için value gerekmiyor
        if (op == FilterOperator.IsNull || op == FilterOperator.IsNotNull)
        {
            return new FilterExpression(propertyName, op);
        }

        // Diğer operatörlerde value gerekli
        if (parts.Length < 3)
            throw new FilterParsingException($"Operator '{operatorStr}' için değer gerekli");

        var valueStr = string.Join(ExpressionSeparator, parts.Skip(2)).Trim();
        var values = valueStr.Split(ValueSeparator)
            .Select(v => v.Trim())
            .ToList();

        return new FilterExpression(propertyName, op, values.ToArray());
    }

    /// <summary>
    /// String operatörü FilterOperator'e dönüştür
    /// </summary>
    private bool TryParseOperator(string operatorStr, out FilterOperator result)
    {
        result = operatorStr switch
        {
            "eq" => FilterOperator.Equals,
            "neq" => FilterOperator.NotEquals,
            "gt" => FilterOperator.GreaterThan,
            "gte" => FilterOperator.GreaterOrEqual,
            "lt" => FilterOperator.LessThan,
            "lte" => FilterOperator.LessOrEqual,
            "contains" => FilterOperator.Contains,
            "startswith" => FilterOperator.StartsWith,
            "endswith" => FilterOperator.EndsWith,
            "between" => FilterOperator.Between,
            "in" => FilterOperator.In,
            "isnull" => FilterOperator.IsNull,
            "notnull" => FilterOperator.IsNotNull,
            _ => FilterOperator.Equals
        };

        return true;
    }

    /// <summary>
    /// İki predicate'i combine et (AND/OR)
    /// </summary>
    private Expression<Func<T, bool>> CombinePredicates(
        Expression<Func<T, bool>> left,
        Expression<Func<T, bool>> right,
        ExpressionType combineType)
    {
        var parameter = Expression.Parameter(typeof(T), "x");

        var leftInvoked = Expression.Invoke(left, parameter);
        var rightInvoked = Expression.Invoke(right, parameter);

        var combined = Expression.MakeBinary(combineType, leftInvoked, rightInvoked);

        return Expression.Lambda<Func<T, bool>>(combined, parameter);
    }
}