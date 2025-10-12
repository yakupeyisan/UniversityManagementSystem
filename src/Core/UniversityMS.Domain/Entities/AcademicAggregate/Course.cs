using UniversityMS.Domain.Entities.Common;
using UniversityMS.Domain.Enums;
using UniversityMS.Domain.Exceptions;

namespace UniversityMS.Domain.Entities.AcademicAggregate;

public class Course : AuditableEntity, ISoftDelete
{
    public string Name { get; private set; }
    public string Code { get; private set; }
    public string? Description { get; private set; }
    public Guid DepartmentId { get; private set; }
    public CourseType CourseType { get; private set; }
    public int TheoreticalHours { get; private set; }
    public int PracticalHours { get; private set; }
    public int ECTS { get; private set; }
    public int NationalCredit { get; private set; }
    public int? Semester { get; private set; } // Hangi dönemde verilir (1-8)
    public EducationLevel EducationLevel { get; private set; }
    public bool IsActive { get; private set; }

    // Soft Delete
    public bool IsDeleted { get; set; }
    public DateTime? DeletedAt { get; set; }
    public string? DeletedBy { get; set; }

    // Navigation Properties
    public Department Department { get; private set; } = null!;

    private readonly List<Prerequisite> _prerequisites = new();
    public IReadOnlyCollection<Prerequisite> Prerequisites => _prerequisites.AsReadOnly();

    private Course() { } // EF Core

    private Course(
        string name, string code, Guid departmentId, CourseType courseType,
        int theoreticalHours, int practicalHours, int ects, int nationalCredit,
        EducationLevel educationLevel, int? semester = null, string? description = null)
        : base()
    {
        Name = name;
        Code = code;
        DepartmentId = departmentId;
        CourseType = courseType;
        TheoreticalHours = theoreticalHours;
        PracticalHours = practicalHours;
        ECTS = ects;
        NationalCredit = nationalCredit;
        EducationLevel = educationLevel;
        Semester = semester;
        Description = description;
        IsActive = true;
    }

    public static Course Create(
        string name, string code, Guid departmentId, CourseType courseType,
        int theoreticalHours, int practicalHours, int ects, int nationalCredit,
        EducationLevel educationLevel, int? semester = null, string? description = null)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new DomainException("Ders adı boş olamaz.");

        if (string.IsNullOrWhiteSpace(code))
            throw new DomainException("Ders kodu boş olamaz.");

        if (departmentId == Guid.Empty)
            throw new DomainException("Bölüm seçilmelidir.");

        if (theoreticalHours < 0 || practicalHours < 0)
            throw new DomainException("Ders saatleri negatif olamaz.");

        if (ects <= 0)
            throw new DomainException("ECTS en az 1 olmalıdır.");

        if (nationalCredit < 0)
            throw new DomainException("Ulusal kredi negatif olamaz.");

        if (semester.HasValue && (semester < 1 || semester > 8))
            throw new DomainException("Dönem 1-8 arasında olmalıdır.");

        return new Course(
            name.Trim(), code.Trim().ToUpperInvariant(), departmentId, courseType,
            theoreticalHours, practicalHours, ects, nationalCredit,
            educationLevel, semester, description?.Trim());
    }

    /// <summary>
    /// Dersi aktif hale getirir
    /// </summary>
    public void Activate()
    {
        if (IsDeleted)
            throw new DomainException("Silinmiş ders aktif edilemez.");

        IsActive = true;
    }

    /// <summary>
    /// Dersi pasif hale getirir
    /// </summary>
    public void Deactivate()
    {
        IsActive = false;
    }

    /// <summary>
    /// Dersi siler (soft delete)
    /// </summary>
    public void Delete(string? deletedBy = null)
    {
        IsDeleted = true;
        DeletedAt = DateTime.UtcNow;
        IsActive = false;
        DeletedBy = deletedBy;
    }

    /// <summary>
    /// Silinen dersi geri yükler
    /// </summary>
    public void Restore()
    {
        if (!IsDeleted)
            throw new DomainException("Ders zaten aktif durumda.");

        IsDeleted = false;
        DeletedAt = null;
    }

    /// <summary>
    /// Ders bilgilerini günceller
    /// </summary>
    public void Update(
        string name,
        int theoreticalHours,
        int practicalHours,
        int ects,
        int nationalCredit,
        int? semester = null,
        string? description = null)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new DomainException("Ders adı boş olamaz.");

        if (theoreticalHours < 0)
            throw new DomainException("Teorik saat negatif olamaz.");

        if (practicalHours < 0)
            throw new DomainException("Uygulama saati negatif olamaz.");

        if (ects <= 0)
            throw new DomainException("ECTS en az 1 olmalıdır.");

        if (semester.HasValue && (semester < 1 || semester > 8))
            throw new DomainException("Dönem 1-8 arasında olmalıdır.");

        Name = name;
        TheoreticalHours = theoreticalHours;
        PracticalHours = practicalHours;
        ECTS = ects;
        NationalCredit = nationalCredit;
        Semester = semester;
        Description = description;
    }

    /// <summary>
    /// Derse ön koşul ekler
    /// </summary>
    public void AddPrerequisite(Guid prerequisiteCourseId)
    {
        if (Id == prerequisiteCourseId)
            throw new DomainException("Bir ders kendisinin ön koşulu olamaz.");

        if (_prerequisites.Any(p => p.PrerequisiteCourseId == prerequisiteCourseId))
            throw new DomainException("Bu ön koşul zaten eklenmiş.");

        var prerequisite = Prerequisite.Create(Id, prerequisiteCourseId);
        _prerequisites.Add(prerequisite);
    }

    /// <summary>
    /// Dersten ön koşul kaldırır
    /// </summary>
    public void RemovePrerequisite(Guid prerequisiteCourseId)
    {
        var prerequisite = _prerequisites.FirstOrDefault(p => p.PrerequisiteCourseId == prerequisiteCourseId);

        if (prerequisite == null)
            throw new DomainException("Belirtilen ön koşul bulunamadı.");

        _prerequisites.Remove(prerequisite);
    }

    /// <summary>
    /// Toplam haftalık ders saatini hesaplar
    /// </summary>
    public int GetTotalWeeklyHours()
    {
        return TheoreticalHours + PracticalHours;
    }

    /// <summary>
    /// Dersin zorunlu mu seçmeli mi olduğunu belirler
    /// </summary>
    public bool IsCompulsory()
    {
        return CourseType == CourseType.Compulsory;
    }

    /// <summary>
    /// Dersin ön koşulları var mı kontrol eder
    /// </summary>
    public bool HasPrerequisites()
    {
        return _prerequisites.Any();
    }

    /// <summary>
    /// Belirli bir ön koşulun var olup olmadığını kontrol eder
    /// </summary>
    public bool HasPrerequisite(Guid prerequisiteCourseId)
    {
        return _prerequisites.Any(p => p.PrerequisiteCourseId == prerequisiteCourseId);
    }

}