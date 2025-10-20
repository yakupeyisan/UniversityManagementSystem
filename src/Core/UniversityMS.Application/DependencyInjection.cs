using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using UniversityMS.Application.Common.Behaviours;

namespace UniversityMS.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(
        this IServiceCollection services)
    {
        services.AddAutoMapper();
        services.AddMediatR();
        services.AddValidators();

        return services;
    }

    private static IServiceCollection AddAutoMapper(
        this IServiceCollection services)
    {
        services.AddAutoMapper(cfg => { }, typeof(DependencyInjection).Assembly);
        return services;
    }

    private static IServiceCollection AddMediatR(
        this IServiceCollection services)
    {
        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssembly(typeof(DependencyInjection).Assembly);

            // Pipeline Behaviors (Sıra önemli!)
            // 1. Validation
            cfg.AddOpenBehavior(typeof(ValidationPipelineBehavior<,>));

            // 2. Logging
            cfg.AddOpenBehavior(typeof(LoggingPipelineBehavior<,>));

            // 3. Caching (en son, çünkü cache verisi döndürebilir)
            cfg.AddOpenBehavior(typeof(CachingPipelineBehavior<,>));
        });

        return services;
    }

    private static IServiceCollection AddValidators(
        this IServiceCollection services)
    {
        services.AddValidatorsFromAssembly(typeof(DependencyInjection).Assembly);
        return services;
    }
}