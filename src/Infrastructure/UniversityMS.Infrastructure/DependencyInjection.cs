using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using UniversityMS.Application.Common.Interfaces;
using UniversityMS.Domain.Interfaces;
using UniversityMS.Infrastructure.Identity;
using UniversityMS.Infrastructure.Persistence;
using UniversityMS.Infrastructure.Persistence.Repositories;
using UniversityMS.Infrastructure.Services;

namespace UniversityMS.Infrastructure;


public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // Database
        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlServer(
                configuration.GetConnectionString("DefaultConnection"),
                b => b.MigrationsAssembly(typeof(ApplicationDbContext).Assembly.FullName)));

        services.AddScoped<IApplicationDbContext>(provider =>
            provider.GetRequiredService<ApplicationDbContext>());

        // Unit of Work
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        // Repositories
        services.AddScoped(typeof(IRepository<>), typeof(Repository<>));

        // Identity & JWT
        services.Configure<JwtSettings>(configuration.GetSection("JwtSettings"));
        services.AddScoped<IJwtTokenGenerator, JwtTokenGenerator>();
        services.AddScoped<IPasswordHasher, PasswordHasher>();
        // Current User Service
        services.AddScoped<ICurrentUserService, CurrentUserService>();

        // Authorization Service
        services.AddScoped<IAuthorizationService, AuthorizationService>();

        // Services
        services.AddTransient<IDateTime, DateTimeService>();
        services.AddTransient<IEmailService, EmailService>();
        services.AddTransient<ISmsService, SmsService>();
        services.AddScoped<IAttendanceService, AttendanceService>();
        services.AddScoped<IScheduleConflictService, ScheduleConflictService>();
        services.AddScoped<IGradeCalculationService, GradeCalculationService>();
        services.AddScoped<IEnrollmentValidationService, EnrollmentValidationService>();
        services.AddScoped<ITaxCalculationService, TaxCalculationService>();
        services.AddScoped<ISGKCalculationService, SGKCalculationService>();
        services.AddScoped<ICacheService, RedisCacheService>();

        return services;
    }
}