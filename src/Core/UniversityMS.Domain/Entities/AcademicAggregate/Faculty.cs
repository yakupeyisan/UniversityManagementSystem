using UniversityMS.Domain.Entities.Common;
using UniversityMS.Domain.Enums;
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

    public void Delete(string deletedBy)
    {
        IsDeleted = true;
        DeletedAt = DateTime.UtcNow;
        DeletedBy = deletedBy;
        IsActive = false;
    }

    public void Restore()
    {
        IsDeleted = false;
        DeletedAt = null;
        DeletedBy = null;
        IsActive = true;
    }
}

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

    public void Delete(string deletedBy)
    {
        IsDeleted = true;
        DeletedAt = DateTime.UtcNow;
        DeletedBy = deletedBy;
        IsActive = false;
    }

    public void Restore()
    {
        IsDeleted = false;
        DeletedAt = null;
        DeletedBy = null;
        IsActive = true;
    }
}
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

    public void Update(string name, int theoreticalHours, int practicalHours,
        int ects, int nationalCredit, int? semester, string? description)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new DomainException("Ders adı boş olamaz.");

        if (theoreticalHours < 0 || practicalHours < 0)
            throw new DomainException("Ders saatleri negatif olamaz.");

        if (ects <= 0)
            throw new DomainException("ECTS en az 1 olmalıdır.");

        if (semester.HasValue && (semester < 1 || semester > 8))
            throw new DomainException("Dönem 1-8 arasında olmalıdır.");

        Name = name.Trim();
        TheoreticalHours = theoreticalHours;
        PracticalHours = practicalHours;
        ECTS = ects;
        NationalCredit = nationalCredit;
        Semester = semester;
        Description = description?.Trim();
    }

    public void AddPrerequisite(Guid prerequisiteCourseId)
    {
        if (prerequisiteCourseId == Guid.Empty)
            throw new DomainException("Geçersiz ön koşul ders ID.");

        if (prerequisiteCourseId == Id)
            throw new DomainException("Ders kendisinin ön koşulu olamaz.");

        if (_prerequisites.Any(p => p.PrerequisiteCourseId == prerequisiteCourseId))
            throw new DomainException("Bu ön koşul zaten ekli.");

        _prerequisites.Add(Prerequisite.Create(Id, prerequisiteCourseId));
    }

    public void RemovePrerequisite(Guid prerequisiteCourseId)
    {
        var prerequisite = _prerequisites.FirstOrDefault(p => p.PrerequisiteCourseId == prerequisiteCourseId);
        if (prerequisite != null)
            _prerequisites.Remove(prerequisite);
    }

    public int GetTotalWeeklyHours() => TheoreticalHours + PracticalHours;

    public void Activate() => IsActive = true;
    public void Deactivate() => IsActive = false;

    public void Delete(string deletedBy)
    {
        IsDeleted = true;
        DeletedAt = DateTime.UtcNow;
        DeletedBy = deletedBy;
        IsActive = false;
    }

    public void Restore()
    {
        IsDeleted = false;
        DeletedAt = null;
        DeletedBy = null;
        IsActive = true;
    }
}

public class Prerequisite : BaseEntity
{
    public Guid CourseId { get; private set; }
    public Guid PrerequisiteCourseId { get; private set; }

    // Navigation Properties
    public Course Course { get; private set; } = null!;
    public Course PrerequisiteCourse { get; private set; } = null!;

    private Prerequisite() { } // EF Core

    private Prerequisite(Guid courseId, Guid prerequisiteCourseId)
        : base()
    {
        CourseId = courseId;
        PrerequisiteCourseId = prerequisiteCourseId;
    }

    public static Prerequisite Create(Guid courseId, Guid prerequisiteCourseId)
    {
        return new Prerequisite(courseId, prerequisiteCourseId);
    }
}

public class Curriculum : AuditableEntity
{
    public string Name { get; private set; }
    public Guid DepartmentId { get; private set; }
    public EducationLevel EducationLevel { get; private set; }
    public int StartYear { get; private set; }
    public int? EndYear { get; private set; }
    public bool IsActive { get; private set; }
    public int TotalRequiredECTS { get; private set; }
    public int TotalRequiredNationalCredit { get; private set; }

    // Navigation Properties
    public Department Department { get; private set; } = null!;

    private readonly List<CurriculumCourse> _curriculumCourses = new();
    public IReadOnlyCollection<CurriculumCourse> CurriculumCourses => _curriculumCourses.AsReadOnly();

    private Curriculum() { } // EF Core

    private Curriculum(string name, Guid departmentId, EducationLevel educationLevel,
        int startYear, int totalRequiredECTS, int totalRequiredNationalCredit)
        : base()
    {
        Name = name;
        DepartmentId = departmentId;
        EducationLevel = educationLevel;
        StartYear = startYear;
        TotalRequiredECTS = totalRequiredECTS;
        TotalRequiredNationalCredit = totalRequiredNationalCredit;
        IsActive = true;
    }

    public static Curriculum Create(string name, Guid departmentId, EducationLevel educationLevel,
        int startYear, int totalRequiredECTS, int totalRequiredNationalCredit)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new DomainException("Müfredat adı boş olamaz.");

        if (departmentId == Guid.Empty)
            throw new DomainException("Bölüm seçilmelidir.");

        if (startYear < 2000 || startYear > 2100)
            throw new DomainException("Geçersiz başlangıç yılı.");

        if (totalRequiredECTS <= 0)
            throw new DomainException("Toplam ECTS 0'dan büyük olmalıdır.");

        return new Curriculum(name.Trim(), departmentId, educationLevel,
            startYear, totalRequiredECTS, totalRequiredNationalCredit);
    }

    public void AddCourse(Guid courseId, int semester, bool isCompulsory)
    {
        if (_curriculumCourses.Any(cc => cc.CourseId == courseId))
            throw new DomainException("Bu ders zaten müfredatta var.");

        if (semester < 1 || semester > 8)
            throw new DomainException("Dönem 1-8 arasında olmalıdır.");

        _curriculumCourses.Add(CurriculumCourse.Create(Id, courseId, semester, isCompulsory));
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
}

public class CurriculumCourse : BaseEntity
{
    public Guid CurriculumId { get; private set; }
    public Guid CourseId { get; private set; }
    public int Semester { get; private set; } // Hangi dönemde alınmalı
    public bool IsCompulsory { get; private set; } // Zorunlu mu, seçmeli mi

    // Navigation Properties
    public Curriculum Curriculum { get; private set; } = null!;
    public Course Course { get; private set; } = null!;

    private CurriculumCourse() { } // EF Core

    private CurriculumCourse(Guid curriculumId, Guid courseId, int semester, bool isCompulsory)
        : base()
    {
        CurriculumId = curriculumId;
        CourseId = courseId;
        Semester = semester;
        IsCompulsory = isCompulsory;
    }

    public static CurriculumCourse Create(Guid curriculumId, Guid courseId, int semester, bool isCompulsory)
    {
        return new CurriculumCourse(curriculumId, courseId, semester, isCompulsory);
    }

    public void UpdateSemester(int semester)
    {
        if (semester < 1 || semester > 8)
            throw new DomainException("Dönem 1-8 arasında olmalıdır.");

        Semester = semester;
    }

    public void SetCompulsory(bool isCompulsory)
    {
        IsCompulsory = isCompulsory;
    }
}