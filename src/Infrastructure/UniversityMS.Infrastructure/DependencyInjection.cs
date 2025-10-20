using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using UniversityMS.Application.Common.Interfaces;
using UniversityMS.Domain.Interfaces;
using UniversityMS.Infrastructure.Identity;
using UniversityMS.Infrastructure.Persistence;
using UniversityMS.Infrastructure.Persistence.Repositories;
using UniversityMS.Infrastructure.Services;
using StackExchange.Redis;
using UniversityMS.Infrastructure.Configuration;

namespace UniversityMS.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services, IConfiguration configuration)
    {
        services.AddPersistence(configuration);
        services.AddCaching(configuration);
        services.AddExternalServices(configuration);

        return services;
    }

    private static IServiceCollection AddPersistence(
        this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException("DefaultConnection not configured");

        services.AddDbContext<ApplicationDbContext>((sp, options) =>
        {
            options.UseSqlServer(connectionString, sqlOptions =>
            {
                sqlOptions.MigrationsAssembly(typeof(ApplicationDbContext).Assembly.FullName);
                sqlOptions.CommandTimeout(30);
                sqlOptions.EnableRetryOnFailure(maxRetryCount: 3);
            });

            // Development'ta query logging
            if (Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Development")
            {
                options.LogTo(Console.WriteLine, LogLevel.Information);
            }
        });

        // Generic Repository Pattern
        services.AddScoped(typeof(IRepository<>), typeof(Repository<>));

        // Unit of Work
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        return services;
    }

    private static IServiceCollection AddCaching(
        this IServiceCollection services, IConfiguration configuration)
    {
        var redisConnection = configuration.GetConnectionString("Redis")
            ?? throw new InvalidOperationException("Redis connection not configured");

        // Redis Cache
        services.AddStackExchangeRedisCache(options =>
        {
            options.Configuration = redisConnection;
            options.InstanceName = "UniversityManagementSystem";
        });

        // Custom Cache Service
        services.AddScoped<ICacheService, RedisCacheService>();

        return services;
    }

    private static IServiceCollection AddExternalServices(
        this IServiceCollection services, IConfiguration configuration)
    {
        // Email Service
        services.Configure<EmailSettings>(configuration.GetSection("Email"));
        services.AddScoped<IEmailService, EmailService>();

        // SMS Service
        services.Configure<SmsSettings>(configuration.GetSection("Sms"));
        services.AddScoped<ISmsService, SmsService>();

        // File Storage
        services.Configure<StorageSettings>(configuration.GetSection("Storage"));
        services.AddScoped<IFileStorageService, LocalFileStorageService>();

        return services;
    }
}
//public static class DependencyInjection
//{
//    public static IServiceCollection AddInfrastructure(
//        this IServiceCollection services,
//        IConfiguration configuration)
//    {
//        // Database
//        services.AddDbContext<ApplicationDbContext>(options =>
//            options.UseSqlServer(
//                configuration.GetConnectionString("DefaultConnection"),
//                b => b.MigrationsAssembly(typeof(ApplicationDbContext).Assembly.FullName)));

//        services.AddScoped<IApplicationDbContext>(provider =>
//            provider.GetRequiredService<ApplicationDbContext>());

//        // Unit of Work
//        services.AddScoped<IUnitOfWork, UnitOfWork>();

//        // Repositories
//        services.AddScoped(typeof(IRepository<>), typeof(Repository<>));

//        // Identity & JWT
//        services.Configure<JwtSettings>(configuration.GetSection("JwtSettings"));
//        services.AddScoped<IJwtTokenGenerator, JwtTokenGenerator>();
//        services.AddScoped<IPasswordHasher, PasswordHasher>();
//        // Current User Service
//        services.AddScoped<ICurrentUserService, CurrentUserService>();

//        // Authorization Service
//        services.AddScoped<IAuthorizationService, AuthorizationService>();

//        // Services
//        services.AddTransient<IDateTime, DateTimeService>();
//        services.AddTransient<IEmailService, EmailService>();
//        services.AddTransient<ISmsService, SmsService>();
//        services.AddScoped<IAttendanceService, AttendanceService>();
//        services.AddScoped<IScheduleConflictService, ScheduleConflictService>();
//        services.AddScoped<IGradeCalculationService, GradeCalculationService>();
//        services.AddScoped<IEnrollmentValidationService, EnrollmentValidationService>();
//        services.AddScoped<ITaxCalculationService, TaxCalculationService>();
//        services.AddScoped<ISGKCalculationService, SGKCalculationService>();
//        services.AddScoped<ICacheService, RedisCacheService>();

//        return services;
//    }
//}