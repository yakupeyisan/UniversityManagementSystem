using System.Linq.Expressions;
using UniversityMS.Domain.Filters;

namespace UniversityMS.Domain.Specifications;

/// <summary>
/// Filter string'ini accept eden base specification
/// Tüm list spec'leri bu'dan inherit etmeli
/// </summary>
public abstract class BaseFilteredSpecification<T> : BaseSpecification<T> where T : class
{
    protected BaseFilteredSpecification() { }
    protected BaseFilteredSpecification(Expression<Func<T,bool>> filter) : base(filter) { }

    protected BaseFilteredSpecification(string? filterString, IFilterParser<T> filterParser)
        : base(filterString != null ? filterParser.Parse(filterString) : x => true)
    {
    }

    protected BaseFilteredSpecification(
        string? filterString,
        IFilterParser<T> filterParser,
        int pageNumber,
        int pageSize)
        : base(filterString != null ? filterParser.Parse(filterString) : x => true)
    {
        ApplyPaging((pageNumber - 1) * pageSize, pageSize);
    }
}