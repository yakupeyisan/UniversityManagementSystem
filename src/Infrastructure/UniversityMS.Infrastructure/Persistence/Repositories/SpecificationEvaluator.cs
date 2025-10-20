using Microsoft.EntityFrameworkCore;
using UniversityMS.Domain.Entities.Common;
using UniversityMS.Domain.Interfaces;

namespace UniversityMS.Infrastructure.Persistence.Repositories;

public class SpecificationEvaluator<T> where T : BaseEntity
{
    public static IQueryable<T> GetQuery(IQueryable<T> inputQuery, ISpecification<T> specification)
    {
        var query = inputQuery;

        if (specification.Criteria != null)
        {
            query = query.Where(specification.Criteria);
        }

        query = specification.Includes.Aggregate(query,
            (current, include) => current.Include(include));

        query = specification.IncludeStrings.Aggregate(query,
            (current, include) => current.Include(include));

        if (specification.OrderBy != null)
        {
            query = query.OrderBy(specification.OrderBy);

            if (specification.OrderByDescriptors != null)
            {
                for (int i = 0; i < specification.OrderByDescriptors.Count; i++)
                {
                    if (specification.IsOrderByDescending[i])
                        query = ((IOrderedQueryable<T>)query).ThenByDescending(specification.OrderByDescriptors[i]);
                    else
                        query = ((IOrderedQueryable<T>)query).ThenBy(specification.OrderByDescriptors[i]);
                }
            }
        }
        else if (specification.OrderByDescending != null)
        {
            query = query.OrderByDescending(specification.OrderByDescending);

            if (specification.OrderByDescriptors != null)
            {
                for (int i = 0; i < specification.OrderByDescriptors.Count; i++)
                {
                    if (specification.IsOrderByDescending[i])
                        query = ((IOrderedQueryable<T>)query).ThenByDescending(specification.OrderByDescriptors[i]);
                    else
                        query = ((IOrderedQueryable<T>)query).ThenBy(specification.OrderByDescriptors[i]);
                }
            }
        }

        if (specification.IsPagingEnabled)
        {
            query = query.Skip(specification.Skip).Take(specification.Take);
        }

        return query;
    }
}