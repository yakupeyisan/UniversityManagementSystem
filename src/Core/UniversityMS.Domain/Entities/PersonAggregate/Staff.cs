using UniversityMS.Domain.Entities.AcademicAggregate;
using UniversityMS.Domain.Entities.EnrollmentAggregate;
using UniversityMS.Domain.Entities.ScheduleAggregate;
using UniversityMS.Domain.Enums;
using UniversityMS.Domain.Exceptions;
using UniversityMS.Domain.ValueObjects;

namespace UniversityMS.Domain.Entities.PersonAggregate;

public class Staff : Person
{
    public string EmployeeNumber { get; private set; }
    public Guid? DepartmentId { get; private set; }
    public AcademicTitle? AcademicTitle { get; private set; }
    public string JobTitle { get; private set; }
    public DateTime HireDate { get; private set; }
    public DateTime? TerminationDate { get; private set; }
    public bool IsActive { get; private set; }
    // Akademik personel için
    public int? WeeklyWorkload { get; private set; } // Haftalık ders saati
    public int? AdviseeCount { get; private set; } // Danışman öğrenci sayısı


    // Bölüm başkanı olduğu departmanlar
    private readonly List<Department> _managedDepartments = new();
    public IReadOnlyCollection<Department> ManagedDepartments
        => _managedDepartments.AsReadOnly();

    // Giriş yaptığı ders oturumları
    private readonly List<CourseSession> _courseSessions = new();
    public IReadOnlyCollection<CourseSession> CourseSessions
        => _courseSessions.AsReadOnly();

    // Girdiği notlar
    private readonly List<Grade> _gradesEntered = new();
    public IReadOnlyCollection<Grade> GradesEntered
        => _gradesEntered.AsReadOnly();

    private Staff() { } // EF Core için

    private Staff(
        string firstName, string lastName, string nationalId, DateTime birthDate,
        Gender gender, Email email, PhoneNumber phoneNumber,
        string employeeNumber, string jobTitle, DateTime hireDate,
        Guid? departmentId = null, AcademicTitle? academicTitle = null)
        : base(firstName, lastName, nationalId, birthDate, gender, email, phoneNumber)
    {
        EmployeeNumber = employeeNumber ?? throw new ArgumentNullException(nameof(employeeNumber));
        JobTitle = jobTitle ?? throw new ArgumentNullException(nameof(jobTitle));
        HireDate = hireDate;
        DepartmentId = departmentId;
        AcademicTitle = academicTitle;
        IsActive = true;
        WeeklyWorkload = 0;
        AdviseeCount = 0;
    }

    public static Staff Create(
        string firstName, string lastName, string nationalId, DateTime birthDate,
        Gender gender, Email email, PhoneNumber phoneNumber,
        string employeeNumber, string jobTitle, DateTime hireDate,
        Guid? departmentId = null, AcademicTitle? academicTitle = null)
    {
        if (string.IsNullOrWhiteSpace(employeeNumber))
            throw new DomainException("Personel numarası boş olamaz.");

        if (string.IsNullOrWhiteSpace(jobTitle))
            throw new DomainException("İş unvanı boş olamaz.");

        if (hireDate > DateTime.UtcNow)
            throw new DomainException("İşe giriş tarihi gelecekte olamaz.");

        return new Staff(firstName, lastName, nationalId, birthDate, gender, email, phoneNumber,
            employeeNumber, jobTitle, hireDate, departmentId, academicTitle);
    }
    public void UpdateJobTitle(string newJobTitle)
    {
        if (string.IsNullOrWhiteSpace(newJobTitle))
            throw new DomainException("İş unvanı boş olamaz.");

        JobTitle = newJobTitle.Trim();
    }

    public void UpdateAcademicTitle(AcademicTitle newTitle)
    {
        AcademicTitle = newTitle;
    }

    public void AssignDepartment(Guid departmentId)
    {
        if (departmentId == Guid.Empty)
            throw new DomainException("Geçersiz bölüm ID.");

        DepartmentId = departmentId;
    }

    public void UpdateWorkload(int weeklyHours)
    {
        if (weeklyHours < 0)
            throw new DomainException("Haftalık ders saati negatif olamaz.");

        if (weeklyHours > 40)
            throw new DomainException("Haftalık ders saati 40'ı geçemez.");

        WeeklyWorkload = weeklyHours;
    }

    public void UpdateAdviseeCount(int count)
    {
        if (count < 0)
            throw new DomainException("Danışman öğrenci sayısı negatif olamaz.");

        AdviseeCount = count;
    }

    public void Activate()
    {
        IsActive = true;
        TerminationDate = null;
    }

    public void Terminate(DateTime terminationDate)
    {
        if (terminationDate < HireDate)
            throw new DomainException("İşten çıkış tarihi, işe giriş tarihinden önce olamaz.");

        IsActive = false;
        TerminationDate = terminationDate;
    }

    public bool IsAcademicStaff()
    {
        return AcademicTitle.HasValue;
    }

    public int GetYearsOfService()
    {
        var endDate = TerminationDate ?? DateTime.UtcNow;
        return (int)((endDate - HireDate).TotalDays / 365.25);
    }
}