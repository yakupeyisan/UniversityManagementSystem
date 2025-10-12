using UniversityMS.Domain.Entities.Common;
using UniversityMS.Domain.Enums;
using UniversityMS.Domain.Exceptions;

namespace UniversityMS.Domain.Entities.AcademicAggregate;

public class Curriculum : AuditableEntity, ISoftDelete
{
    public string Name { get; private set; }
    public string Code { get; private set; } // YENİ: Müfredat kodu (örn: "COMP-2024")
    public Guid DepartmentId { get; private set; }
    public EducationLevel EducationLevel { get; private set; }
    public string AcademicYear { get; private set; } // YENİ: "2024-2025" formatında
    public int StartYear { get; private set; }
    public int? EndYear { get; private set; }
    public bool IsActive { get; private set; }
    public int TotalECTS { get; private set; } // YENİ: TotalRequiredECTS yerine
    public int TotalRequiredECTS { get; private set; }
    public int TotalRequiredNationalCredit { get; private set; }

    // ISoftDelete implementation
    public bool IsDeleted { get; set; }
    public DateTime? DeletedAt { get; set; }
    public string? DeletedBy { get; set; }

    // Navigation Properties
    public Department Department { get; private set; } = null!;

    private readonly List<CurriculumCourse> _curriculumCourses = new();
    public IReadOnlyCollection<CurriculumCourse> CurriculumCourses => _curriculumCourses.AsReadOnly();

    private Curriculum() { } // EF Core

    private Curriculum(string name, string code, Guid departmentId, EducationLevel educationLevel,
        string academicYear, int totalRequiredECTS, int totalRequiredNationalCredit)
        : base()
    {
        Name = name;
        Code = code;
        DepartmentId = departmentId;
        EducationLevel = educationLevel;
        AcademicYear = academicYear;
        StartYear = int.Parse(academicYear.Split('-')[0]);
        TotalRequiredECTS = totalRequiredECTS;
        TotalECTS = totalRequiredECTS;
        TotalRequiredNationalCredit = totalRequiredNationalCredit;
        IsActive = true;
        IsDeleted = false;
    }

    public static Curriculum Create(string name, string code, Guid departmentId,
        EducationLevel educationLevel, string academicYear,
        int totalRequiredECTS, int totalRequiredNationalCredit)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new DomainException("Müfredat adı boş olamaz.");

        if (string.IsNullOrWhiteSpace(code))
            throw new DomainException("Müfredat kodu boş olamaz.");

        if (string.IsNullOrWhiteSpace(academicYear))
            throw new DomainException("Akademik yıl boş olamaz.");

        if (departmentId == Guid.Empty)
            throw new DomainException("Bölüm seçilmelidir.");

        if (totalRequiredECTS <= 0)
            throw new DomainException("Toplam ECTS 0'dan büyük olmalıdır.");

        return new Curriculum(name.Trim(), code.Trim().ToUpperInvariant(),
            departmentId, educationLevel, academicYear,
            totalRequiredECTS, totalRequiredNationalCredit);
    }

    public void AddCourse(Guid courseId, int semester, CourseType courseType, bool isElective)
    {
        if (_curriculumCourses.Any(cc => cc.CourseId == courseId))
            throw new DomainException("Bu ders zaten müfredatta var.");

        if (semester < 1 || semester > 8)
            throw new DomainException("Dönem 1-8 arasında olmalıdır.");

        _curriculumCourses.Add(CurriculumCourse.Create(Id, courseId, semester, courseType, isElective));
    }

    public void RemoveCourse(Guid courseId)
    {
        var curriculumCourse = _curriculumCourses.FirstOrDefault(cc => cc.CourseId == courseId);
        if (curriculumCourse != null)
            _curriculumCourses.Remove(curriculumCourse);
    }

    public void Activate() => IsActive = true;

    public void Deactivate()
    {
        IsActive = false;
        EndYear = DateTime.UtcNow.Year;
    }

    public void Delete(string? deletedBy = null)
    {
        IsDeleted = true;
        DeletedAt = DateTime.UtcNow;
        IsActive = false;
    }

    public void Restore()
    {
        IsDeleted = false;
        DeletedBy = null;
        DeletedAt = null;
    }
}