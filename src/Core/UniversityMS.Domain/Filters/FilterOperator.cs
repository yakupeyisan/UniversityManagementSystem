namespace UniversityMS.Domain.Filters;

/// <summary>
/// Desteklenen filter operatörleri
/// Örnek: price|gt|100
/// </summary>
public enum FilterOperator
{
    Equals,           // eq
    NotEquals,        // neq
    GreaterThan,      // gt
    GreaterOrEqual,   // gte
    LessThan,         // lt
    LessOrEqual,      // lte
    Contains,         // contains (string)
    StartsWith,       // startswith (string)
    EndsWith,         // endswith (string)
    Between,          // between (value1,value2)
    In,               // in (val1,val2,val3)
    IsNull,           // isnull
    IsNotNull         // notnull
}