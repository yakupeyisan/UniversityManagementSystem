using MediatR;
using Microsoft.Extensions.DependencyInjection;
using FluentValidation;
using System.Reflection;
using UniversityMS.Application.Common.Behaviours;

namespace UniversityMS.Application;
public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        var assembly = Assembly.GetExecutingAssembly();

        // MediatR
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(assembly));

        // AutoMapper
        services.AddAutoMapper(assembly);

        // FluentValidation
        services.AddValidatorsFromAssembly(assembly);

        // Pipeline Behaviours
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehaviour<,>));
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(LoggingBehaviour<,>));
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(PerformanceBehaviour<,>));

        return services;
    }
}