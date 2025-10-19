using UniversityMS.Domain.Entities.Common;
using UniversityMS.Domain.Entities.PersonAggregate;
using UniversityMS.Domain.Exceptions;

namespace UniversityMS.Domain.Entities.AcademicAggregate;

public class Department : AuditableEntity, ISoftDelete
{
    public string Name { get; private set; }
    public string Code { get; private set; }
    public string? Description { get; private set; }
    public Guid FacultyId { get; private set; }
    public Guid? HeadOfDepartmentId { get; private set; }
    public bool IsActive { get; private set; }

    // Soft Delete
    public bool IsDeleted { get; set; }
    public DateTime? DeletedAt { get; set; }
    public string? DeletedBy { get; set; }

    // Navigation Properties
    public Faculty Faculty { get; private set; } = null!;
    public Staff? HeadOfDepartment { get; private set; } 

    private readonly List<Course> _courses = new();
    public IReadOnlyCollection<Course> Courses => _courses.AsReadOnly();

    private Department() { } // EF Core

    private Department(string name, string code, Guid facultyId, string? description = null)
        : base()
    {
        Name = name;
        Code = code;
        FacultyId = facultyId;
        Description = description;
        IsActive = true;
    }

    public static Department Create(string name, string code, Guid facultyId, string? description = null)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new DomainException("Bölüm adı boş olamaz.");

        if (string.IsNullOrWhiteSpace(code))
            throw new DomainException("Bölüm kodu boş olamaz.");

        if (facultyId == Guid.Empty)
            throw new DomainException("Fakülte seçilmelidir.");

        return new Department(name.Trim(), code.Trim().ToUpperInvariant(), facultyId, description?.Trim());
    }

    public void Update(string name, string? description)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new DomainException("Bölüm adı boş olamaz.");

        Name = name.Trim();
        Description = description?.Trim();
    }

    public void AssignHead(Guid headId)
    {
        if (headId == Guid.Empty)
            throw new DomainException("Geçersiz bölüm başkanı ID.");

        HeadOfDepartmentId = headId;
    }

    public void RemoveHead()
    {
        HeadOfDepartmentId = null;
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