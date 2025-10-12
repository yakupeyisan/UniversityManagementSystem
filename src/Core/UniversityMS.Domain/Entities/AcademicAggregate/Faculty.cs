using UniversityMS.Domain.Entities.Common;
using UniversityMS.Domain.Exceptions;

namespace UniversityMS.Domain.Entities.AcademicAggregate;

public class Faculty : AuditableEntity, ISoftDelete
{
    public string Name { get; private set; }
    public string Code { get; private set; }
    public string? Description { get; private set; }
    public Guid? DeanId { get; private set; }
    public bool IsActive { get; private set; }

    // Soft Delete
    public bool IsDeleted { get; set; }
    public DateTime? DeletedAt { get; set; }
    public string? DeletedBy { get; set; }

    // Navigation Properties
    private readonly List<Department> _departments = new();
    public IReadOnlyCollection<Department> Departments => _departments.AsReadOnly();

    private Faculty() { } // EF Core

    private Faculty(string name, string code, string? description = null)
        : base()
    {
        Name = name;
        Code = code;
        Description = description;
        IsActive = true;
    }

    public static Faculty Create(string name, string code, string? description = null)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new DomainException("Fakülte adı boş olamaz.");

        if (string.IsNullOrWhiteSpace(code))
            throw new DomainException("Fakülte kodu boş olamaz.");

        if (code.Length > 10)
            throw new DomainException("Fakülte kodu en fazla 10 karakter olabilir.");

        return new Faculty(name.Trim(), code.Trim().ToUpperInvariant(), description?.Trim());
    }

    public void Update(string name, string? description)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new DomainException("Fakülte adı boş olamaz.");

        Name = name.Trim();
        Description = description?.Trim();
    }
    public void Update(string name, string code, string? description)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new DomainException("Fakülte adı boş olamaz.");

        if (string.IsNullOrWhiteSpace(code))
            throw new DomainException("Fakülte kodu boş olamaz.");

        Name = name.Trim();
        Code = code.Trim().ToUpperInvariant();
        Description = description?.Trim();
    }

    public void AssignDean(Guid deanId)
    {
        if (deanId == Guid.Empty)
            throw new DomainException("Geçersiz dekan ID.");

        DeanId = deanId;
    }

    public void RemoveDean()
    {
        DeanId = null;
    }

    public void Activate() => IsActive = true;
    public void Deactivate() => IsActive = false;

    public void Delete(string? deletedBy)
    {
        IsDeleted = true;
        DeletedAt = DateTime.UtcNow;
        IsActive = false;
        DeletedBy = deletedBy;
    }

    public void Restore()
    {
        IsDeleted = false;
        DeletedAt = null;
        DeletedBy = null;
        IsActive = true;
    }
}