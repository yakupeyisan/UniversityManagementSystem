namespace UniversityMS.Domain.Filters;

/// <summary>
/// Parse edilmiş tek bir filter expression
/// Örnek: price|gt|100 → PropertyName: "price", Operator: gt, Value: "100"
/// </summary>
public class FilterExpression
{
    public string PropertyName { get; set; } = null!;
    public FilterOperator Operator { get; set; }
    public List<string> Values { get; set; } = new();

    public FilterExpression() { }

    public FilterExpression(string propertyName, FilterOperator op, params string[] values)
    {
        PropertyName = propertyName;
        Operator = op;
        Values = values.ToList();
    }
}
