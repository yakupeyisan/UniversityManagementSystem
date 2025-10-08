using System.Linq.Expressions;
using UniversityMS.Domain.Entities.Common;

namespace UniversityMS.Domain.Interfaces;
public interface IAuditableEntity
{
    DateTime CreatedAt { get; set; }
    string? CreatedBy { get; set; }
    DateTime? UpdatedAt { get; set; }
    string? UpdatedBy { get; set; }
}
public interface IDomainEvent
{
    DateTime OccurredOn { get; }
}
public abstract class BaseDomainEvent : IDomainEvent
{
    public DateTime OccurredOn { get; }

    protected BaseDomainEvent()
    {
        OccurredOn = DateTime.UtcNow;
    }
}

public interface IRepository<T> where T : BaseEntity
{
    // Read Operations
    Task<T?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<T>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyList<T>> FindAsync(Expression<Func<T, bool>> predicate,
        CancellationToken cancellationToken = default);
    Task<T?> FirstOrDefaultAsync(Expression<Func<T, bool>> predicate,
        CancellationToken cancellationToken = default);

    // Specification Pattern
    Task<T?> GetBySpecAsync(ISpecification<T> spec, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<T>> ListAsync(ISpecification<T> spec,
        CancellationToken cancellationToken = default);
    Task<int> CountAsync(ISpecification<T> spec, CancellationToken cancellationToken = default);
    Task<bool> AnyAsync(ISpecification<T> spec, CancellationToken cancellationToken = default);

    // Write Operations
    Task<T> AddAsync(T entity, CancellationToken cancellationToken = default);
    Task<IEnumerable<T>> AddRangeAsync(IEnumerable<T> entities,
        CancellationToken cancellationToken = default);
    Task UpdateAsync(T entity, CancellationToken cancellationToken = default);
    Task UpdateRangeAsync(IEnumerable<T> entities, CancellationToken cancellationToken = default);
    Task DeleteAsync(T entity, CancellationToken cancellationToken = default);
    Task DeleteRangeAsync(IEnumerable<T> entities, CancellationToken cancellationToken = default);

    // Pagination
    Task<(IReadOnlyList<T> Items, int TotalCount)> GetPagedAsync(
        int pageNumber, int pageSize,
        Expression<Func<T, bool>>? predicate = null,
        CancellationToken cancellationToken = default);
}
public interface IUnitOfWork : IDisposable
{
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    Task<bool> SaveEntitiesAsync(CancellationToken cancellationToken = default);
    Task BeginTransactionAsync(CancellationToken cancellationToken = default);
    Task CommitTransactionAsync(CancellationToken cancellationToken = default);
    Task RollbackTransactionAsync();
}

public interface ISpecification<T>
{
    Expression<Func<T, bool>>? Criteria { get; }
    List<Expression<Func<T, object>>> Includes { get; }
    List<string> IncludeStrings { get; }
    Expression<Func<T, object>>? OrderBy { get; }
    Expression<Func<T, object>>? OrderByDescending { get; }
    int Take { get; }
    int Skip { get; }
    bool IsPagingEnabled { get; }
}