using Microsoft.Extensions.DependencyInjection;
using UniversityMS.Domain.Filters;

namespace UniversityMS.Application.Common.Extensions;

public static class FilterServiceExtensions
{
    public static IServiceCollection AddFilterParsing(this IServiceCollection services)
    {
        // Generic FilterParser registration
        services.AddScoped(typeof(IFilterParser<>), typeof(FilterParser<>));
        services.AddScoped(typeof(IFilterExpressionBuilder<>), typeof(FilterExpressionBuilder<>));

        return services;
    }
}