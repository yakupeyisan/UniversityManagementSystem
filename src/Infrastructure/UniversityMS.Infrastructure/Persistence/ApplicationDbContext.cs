using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using UniversityMS.Application.Common.Interfaces;
using UniversityMS.Domain.Entities.AcademicAggregate;
using UniversityMS.Domain.Entities.Common;
using UniversityMS.Domain.Entities.EnrollmentAggregate;
using UniversityMS.Domain.Entities.FacilityAggregate;
using UniversityMS.Domain.Entities.HRAggregate;
using UniversityMS.Domain.Entities.IdentityAggregate;
using UniversityMS.Domain.Entities.InventoryAggregate;
using UniversityMS.Domain.Entities.PayrollAggregate;
using UniversityMS.Domain.Entities.PersonAggregate;
using UniversityMS.Domain.Entities.ScheduleAggregate;
using UniversityMS.Domain.Interfaces;

namespace UniversityMS.Infrastructure.Persistence;

public class ApplicationDbContext : DbContext, IApplicationDbContext
{
    private IDbContextTransaction? _transaction;

    // DbSets
    public DbSet<User> Users { get; set; } = null!;
    public DbSet<Role> Roles { get; set; } = null!;
    public DbSet<Permission> Permissions { get; set; } = null!;

    public DbSet<Student> Students { get; set; } = null!;
    public DbSet<Employee> Employees { get; set; } = null!;
    public DbSet<Faculty> Faculties { get; set; } = null!;
    public DbSet<Department> Departments { get; set; } = null!;
    public DbSet<Course> Courses { get; set; } = null!;
    public DbSet<Prerequisite> Prerequisites { get; set; } = null!;
    public DbSet<Curriculum> Curriculums { get; set; } = null!;
    public DbSet<CurriculumCourse> CurriculumCourses { get; set; } = null!;

    public DbSet<Enrollment> Enrollments { get; set; } = null!;
    public DbSet<CourseRegistration> CourseRegistrations { get; set; } = null!;
    public DbSet<Grade> Grades { get; set; } = null!;
    public DbSet<Attendance> Attendances { get; set; } = null!;
    public DbSet<GradeObjection> GradeObjections { get; set; } = null!;

    public DbSet<Schedule> Schedules { get; set; } = null!;
    public DbSet<CourseSession> CourseSessions { get; set; } = null!;

    public DbSet<Classroom> Classrooms { get; set; } = null!;
    public DbSet<Payslip> Payslips { get; set; } = null!;

    public DbSet<Leave> Leaves { get; set; } = null!;
    public DbSet<Contract> Contracts { get; set; } = null!;
    public DbSet<Shift> Shifts { get; set; } = null!;
    public DbSet<Payroll> Payrolls { get; set; } = null!;

    public DbSet<StockItem> StockItems { get; set; } = null!;
    public DbSet<Warehouse> Warehouses { get; set; } = null!;

    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // ✅ Configurations referans ekle
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);

        // ✅ Audit shadows
        foreach (var entity in modelBuilder.Model.GetEntityTypes())
        {
            if (typeof(AuditableEntity).IsAssignableFrom(entity.ClrType))
            {
                modelBuilder
                    .Entity(entity.ClrType)
                    .Property<DateTime>("CreatedAt");

                modelBuilder
                    .Entity(entity.ClrType)
                    .Property<DateTime>("UpdatedAt");
            }
        }
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        // ✅ Audit properties set
        var entries = ChangeTracker.Entries<IAuditableEntity>();

        foreach (var entry in entries)
        {
            if (entry.State == EntityState.Added)
            {
                entry.Entity.CreatedAt = DateTime.UtcNow;
                entry.Entity.UpdatedAt = DateTime.UtcNow;
            }
            else if (entry.State == EntityState.Modified)
            {
                entry.Entity.UpdatedAt = DateTime.UtcNow;
            }
        }

        return await base.SaveChangesAsync(cancellationToken);
    }

    public async Task BeginTransactionAsync(CancellationToken cancellationToken = default)
    {
        _transaction = await Database.BeginTransactionAsync(cancellationToken);
    }

    public async Task CommitTransactionAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            await SaveChangesAsync(cancellationToken);
            await _transaction?.CommitAsync(cancellationToken)!;
        }
        finally
        {
            _transaction?.Dispose();
            _transaction = null;
        }
    }

    public async Task RollbackTransactionAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            await _transaction?.RollbackAsync(cancellationToken)!;
        }
        finally
        {
            _transaction?.Dispose();
            _transaction = null;
        }
    }
}

public class SpecificationEvaluator<T> where T : BaseEntity
{
    public static IQueryable<T> GetQuery(IQueryable<T> inputQuery, ISpecification<T> specification)
    {
        var query = inputQuery;

        // ✅ Apply criteria
        if (specification.Criteria != null)
        {
            query = query.Where(specification.Criteria);
        }

        // ✅ Apply includes
        query = specification.Includes.Aggregate(query,
            (current, include) => current.Include(include));

        // ✅ Apply string-based includes
        query = specification.IncludeStrings.Aggregate(query,
            (current, include) => current.Include(include));

        // ✅ Apply ordering
        if (specification.OrderBy != null)
        {
            query = query.OrderBy(specification.OrderBy);
        }
        else if (specification.OrderByDescending != null)
        {
            query = query.OrderByDescending(specification.OrderByDescending);
        }

        // ✅ Apply paging
        if (specification.IsPagingEnabled)
        {
            if (specification.Skip.HasValue)
            {
                query = query.Skip(specification.Skip.Value);
            }

            if (specification.Take.HasValue)
            {
                query = query.Take(specification.Take.Value);
            }
        }

        return query;
    }
}