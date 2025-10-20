using UniversityMS.Domain.Entities.EnrollmentAggregate;
using UniversityMS.Domain.Enums;
using UniversityMS.Domain.Events;
using UniversityMS.Domain.Exceptions;
using UniversityMS.Domain.ValueObjects;

namespace UniversityMS.Domain.Entities.PersonAggregate;

public class Student : Person
{
    public string StudentNumber { get; private set; }
    public Guid DepartmentId { get; private set; }
    public EducationLevel EducationLevel { get; private set; }
    public int CurrentSemester { get; private set; }
    public StudentStatus Status { get; private set; }
    public double CGPA { get; private set; } // GANO
    public double SGPA { get; private set; } // YANO
    public int TotalCredits { get; private set; }
    public int CompletedCredits { get; private set; }
    public DateTime EnrollmentDate { get; private set; }
    public DateTime? GraduationDate { get; private set; }
    public string? QRCode { get; private set; }
    public string? CardNumber { get; private set; }
    public Money Balance { get; private set; }
    public Guid? AdvisorId { get; private set; }
    public Staff? Advisor { get; private set; }
    private readonly List<Grade> _grades = new();
    public IReadOnlyCollection<Grade> Grades => _grades.AsReadOnly();

    private Student() { } // EF Core için

    private Student(
        string firstName, string lastName, string nationalId, DateTime birthDate,
        Gender gender, Email email, PhoneNumber phoneNumber,
        string studentNumber, Guid departmentId, EducationLevel educationLevel)
        : base(firstName, lastName, nationalId, birthDate, gender, email, phoneNumber)
    {
        StudentNumber = studentNumber ?? throw new ArgumentNullException(nameof(studentNumber));
        DepartmentId = departmentId;
        EducationLevel = educationLevel;
        CurrentSemester = 1;
        Status = StudentStatus.Active;
        CGPA = 0;
        SGPA = 0;
        TotalCredits = 0;
        CompletedCredits = 0;
        EnrollmentDate = DateTime.UtcNow;
        Balance = Money.Zero();

        GenerateQRCode();
        GenerateCardNumber();

        // Domain Event
        AddDomainEvent(new StudentEnrolledEvent(Id, StudentNumber, DepartmentId));
    }

    public static Student Create(
        string firstName, string lastName, string nationalId, DateTime birthDate,
        Gender gender, Email email, PhoneNumber phoneNumber,
        string studentNumber, Guid departmentId, EducationLevel educationLevel)
    {
        if (string.IsNullOrWhiteSpace(studentNumber))
            throw new DomainException("Öğrenci numarası boş olamaz.");

        if (departmentId == Guid.Empty)
            throw new DomainException("Bölüm seçilmelidir.");

        return new Student(firstName, lastName, nationalId, birthDate, gender, email, phoneNumber,
            studentNumber, departmentId, educationLevel);
    }

    private void GenerateQRCode()
    {
        // QR Code: STU_{StudentId}_{Timestamp}
        QRCode = $"STU_{Id}_{DateTimeOffset.UtcNow.ToUnixTimeSeconds()}";
    }

    private void GenerateCardNumber()
    {
        // Card Number: 16 haneli benzersiz numara
        CardNumber = $"{DateTime.UtcNow.Ticks:0000000000000000}";
    }

    public void UpdateGPA(double cgpa, double sgpa)
    {
        if (cgpa < 0 || cgpa > 4.0)
            throw new DomainException("CGPA 0-4.0 arasında olmalıdır.");

        if (sgpa < 0 || sgpa > 4.0)
            throw new DomainException("SGPA 0-4.0 arasında olmalıdır.");

        CGPA = Math.Round(cgpa, 2);
        SGPA = Math.Round(sgpa, 2);
    }

    public void UpdateCredits(int total, int completed)
    {
        if (total < 0 || completed < 0)
            throw new DomainException("Kredi değerleri negatif olamaz.");

        if (completed > total)
            throw new DomainException("Tamamlanan kredi, toplam krediden fazla olamaz.");

        TotalCredits = total;
        CompletedCredits = completed;
    }

    public void IncrementSemester()
    {
        CurrentSemester++;
    }

    public void ChangeStatus(StudentStatus newStatus)
    {
        Status = newStatus;

        if (newStatus == StudentStatus.Graduated)
        {
            GraduationDate = DateTime.UtcNow;
        }
    }

    public void AddBalance(Money amount)
    {
        Balance = Balance.Add(amount);
    }

    public void DeductBalance(Money amount)
    {
        Balance = Balance.Subtract(amount);
    }

    public void BlockCard()
    {
        // Kart bloke işlemi
        Status = StudentStatus.Suspended;
    }

    public void RequestNewCard()
    {
        GenerateQRCode();
        GenerateCardNumber();
    }

    public bool IsEligibleForGraduation()
    {
        // Mezuniyet için gereken minimum CGPA: 2.0
        // Tüm kredilerin tamamlanması gerekli
        return CGPA >= 2.0 && CompletedCredits >= TotalCredits && TotalCredits > 0;
    }
    /// <summary>
    /// Öğrenci durumunu günceller
    /// </summary>
    public void UpdateStatus(StudentStatus status)
    {
        if (IsDeleted)
            throw new DomainException("Silinmiş öğrencinin durumu değiştirilemez.");

        Status = status;
    }

    /// <summary>
    /// Öğrenci kayıt dondurma
    /// </summary>
    public void Freeze()
    {
        if (Status == StudentStatus.Frozen)
            throw new DomainException("Öğrenci zaten kayıt dondurmuş.");

        Status = StudentStatus.Frozen;
    }

    /// <summary>
    /// Kayıt dondurma kaldırma
    /// </summary>
    public void Unfreeze()
    {
        if (Status != StudentStatus.Frozen)
            throw new DomainException("Öğrencinin kayıt dondurmması yok.");

        Status = StudentStatus.Active;
    }

    /// <summary>
    /// Öğrenciyi mezun et
    /// </summary>
    public void Graduate()
    {
        if (Status == StudentStatus.Graduate)
            throw new DomainException("Öğrenci zaten mezun.");

        Status = StudentStatus.Graduate;
    }

    /// <summary>
    /// Öğrencinin CGPA'sını güncelle
    /// </summary>
    public void UpdateCGPA(double cgpa)
    {
        if (cgpa < 0 || cgpa > 4.0)
            throw new DomainException("CGPA 0-4.0 arasında olmalıdır.");

        CGPA = cgpa;
    }

    /// <summary>
    /// Öğrencinin SGPA'sını güncelle
    /// </summary>
    public void UpdateSGPA(double sgpa)
    {
        if (sgpa < 0 || sgpa > 4.0)
            throw new DomainException("SGPA 0-4.0 arasında olmalıdır.");

        SGPA = sgpa;
    }

    /// <summary>
    /// Tamamlanan kredi güncellemesi
    /// </summary>
    public void UpdateCompletedCredits(int credits)
    {
        if (credits < 0)
            throw new DomainException("Kredi negatif olamaz.");

        CompletedCredits = credits;
    }

    /// <summary>
    /// Dönem güncellemesi
    /// </summary>
    public void AdvanceSemester()
    {
        if (CurrentSemester >= 8)
            throw new DomainException("Maksimum dönem sayısına ulaşıldı.");

        CurrentSemester++;
    }
}