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
    public static IServiceCollection AddInfrastructureServices(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // ✅ DbContext registration
        var connectionString = configuration.GetConnectionString("DefaultConnection")
                               ?? throw new InvalidOperationException(
                                   "Connection string 'DefaultConnection' not found in configuration. " +
                                   "Check appsettings.json");

        services.AddDbContext<ApplicationDbContext>(options =>
        {
            options.UseSqlServer(
                connectionString,
                x => x.MigrationsAssembly("UniversityMS.Infrastructure"));
        });

        services.AddScoped<IApplicationDbContext>(provider =>
            provider.GetRequiredService<ApplicationDbContext>());

        // ✅ Repository registration
        services.AddScoped(typeof(IRepository<>), typeof(Repository<>));

        // ✅ UnitOfWork registration
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        return services;
    }
}