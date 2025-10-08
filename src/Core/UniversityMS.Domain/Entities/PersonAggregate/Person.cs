using System.Net;
using UniversityMS.Domain.Entities.Common;
using UniversityMS.Domain.Enums;
using UniversityMS.Domain.Events;
using UniversityMS.Domain.Exceptions;
using UniversityMS.Domain.ValueObjects;

namespace UniversityMS.Domain.Entities.PersonAggregate;

public abstract class Person : AuditableEntity, ISoftDelete
{
    public string FirstName { get; private set; }
    public string LastName { get; private set; }
    public string NationalId { get; private set; }
    public DateTime BirthDate { get; private set; }
    public Gender Gender { get; private set; }
    public Email Email { get; private set; }
    public PhoneNumber PhoneNumber { get; private set; }
    public string? ProfilePhotoUrl { get; private set; }

    // Soft Delete
    public bool IsDeleted { get; set; }
    public DateTime? DeletedAt { get; set; }
    public string? DeletedBy { get; set; }

    // Navigation Properties
    public Address? Address { get; private set; }
    public EmergencyContact? EmergencyContact { get; private set; }

    protected Person() { } // EF Core için

    protected Person(string firstName, string lastName, string nationalId, DateTime birthDate,
        Gender gender, Email email, PhoneNumber phoneNumber)
        : base()
    {
        FirstName = firstName ?? throw new ArgumentNullException(nameof(firstName));
        LastName = lastName ?? throw new ArgumentNullException(nameof(lastName));
        NationalId = nationalId ?? throw new ArgumentNullException(nameof(nationalId));
        BirthDate = birthDate;
        Gender = gender;
        Email = email ?? throw new ArgumentNullException(nameof(email));
        PhoneNumber = phoneNumber ?? throw new ArgumentNullException(nameof(phoneNumber));

        ValidateAge();
        ValidateNationalId();
    }

    private void ValidateAge()
    {
        var age = DateTime.Today.Year - BirthDate.Year;
        if (BirthDate.Date > DateTime.Today.AddYears(-age)) age--;

        if (age < 15)
            throw new DomainException("Yaş 15'ten küçük olamaz.");
    }

    private void ValidateNationalId()
    {
        if (NationalId.Length != 11 || !NationalId.All(char.IsDigit))
            throw new DomainException("TC Kimlik Numarası 11 haneli olmalıdır.");
    }

    public void UpdateBasicInfo(string firstName, string lastName, PhoneNumber phoneNumber)
    {
        FirstName = firstName?.Trim() ?? throw new ArgumentNullException(nameof(firstName));
        LastName = lastName?.Trim() ?? throw new ArgumentNullException(nameof(lastName));
        PhoneNumber = phoneNumber ?? throw new ArgumentNullException(nameof(phoneNumber));
    }

    public void UpdateEmail(Email newEmail)
    {
        Email = newEmail ?? throw new ArgumentNullException(nameof(newEmail));
    }

    public void UpdateProfilePhoto(string photoUrl)
    {
        ProfilePhotoUrl = photoUrl;
    }

    public void SetAddress(Address address)
    {
        Address = address;
    }

    public void SetEmergencyContact(EmergencyContact contact)
    {
        EmergencyContact = contact;
    }

    public void Delete(string deletedBy)
    {
        IsDeleted = true;
        DeletedAt = DateTime.UtcNow;
        DeletedBy = deletedBy;
    }

    public void Restore()
    {
        IsDeleted = false;
        DeletedAt = null;
        DeletedBy = null;
    }

    public int GetAge()
    {
        var age = DateTime.Today.Year - BirthDate.Year;
        if (BirthDate.Date > DateTime.Today.AddYears(-age)) age--;
        return age;
    }

    public string GetFullName() => $"{FirstName} {LastName}";
}

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
}

public class Staff : Person
{
    public string EmployeeNumber { get; private set; }
    public Guid? DepartmentId { get; private set; }
    public AcademicTitle? AcademicTitle { get; private set; }
    public string JobTitle { get; private set; }
    public DateTime HireDate { get; private set; }
    public DateTime? TerminationDate { get; private set; }
    public bool IsActive { get; private set; }
    public Money Balance { get; private set; }
    public string? QRCode { get; private set; }

    // Akademik personel için
    public int? WeeklyWorkload { get; private set; } // Haftalık ders saati
    public int? AdviseeCount { get; private set; } // Danışman öğrenci sayısı

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
        Balance = Money.Zero();
        WeeklyWorkload = 0;
        AdviseeCount = 0;

        GenerateQRCode();
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

    private void GenerateQRCode()
    {
        // QR Code: STF_{StaffId}_{Timestamp}
        QRCode = $"STF_{Id}_{DateTimeOffset.UtcNow.ToUnixTimeSeconds()}";
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

    public void AddBalance(Money amount)
    {
        Balance = Balance.Add(amount);
    }

    public void DeductBalance(Money amount)
    {
        Balance = Balance.Subtract(amount);
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

public class Address : BaseEntity
{
    public string Street { get; private set; }
    public string? BuildingNo { get; private set; }
    public string? ApartmentNo { get; private set; }
    public string District { get; private set; }
    public string City { get; private set; }
    public string PostalCode { get; private set; }
    public string Country { get; private set; }

    private Address() { } // EF Core için

    private Address(string street, string district, string city, string postalCode,
        string country, string? buildingNo = null, string? apartmentNo = null)
        : base()
    {
        Street = street;
        District = district;
        City = city;
        PostalCode = postalCode;
        Country = country;
        BuildingNo = buildingNo;
        ApartmentNo = apartmentNo;
    }

    public static Address Create(string street, string district, string city,
        string postalCode, string country = "Türkiye",
        string? buildingNo = null, string? apartmentNo = null)
    {
        if (string.IsNullOrWhiteSpace(street))
            throw new DomainException("Sokak bilgisi boş olamaz.");

        if (string.IsNullOrWhiteSpace(district))
            throw new DomainException("İlçe bilgisi boş olamaz.");

        if (string.IsNullOrWhiteSpace(city))
            throw new DomainException("İl bilgisi boş olamaz.");

        if (string.IsNullOrWhiteSpace(postalCode))
            throw new DomainException("Posta kodu boş olamaz.");

        return new Address(street.Trim(), district.Trim(), city.Trim(),
            postalCode.Trim(), country.Trim(), buildingNo?.Trim(), apartmentNo?.Trim());
    }

    public string GetFullAddress()
    {
        var parts = new List<string> { Street };

        if (!string.IsNullOrWhiteSpace(BuildingNo))
            parts.Add($"No: {BuildingNo}");

        if (!string.IsNullOrWhiteSpace(ApartmentNo))
            parts.Add($"Daire: {ApartmentNo}");

        parts.Add(District);
        parts.Add($"{City} {PostalCode}");
        parts.Add(Country);

        return string.Join(", ", parts);
    }
}

public class EmergencyContact : BaseEntity
{
    public string FullName { get; private set; }
    public string Relationship { get; private set; }
    public PhoneNumber PhoneNumber { get; private set; }
    public PhoneNumber? AlternativePhone { get; private set; }

    private EmergencyContact() { } // EF Core için

    private EmergencyContact(string fullName, string relationship,
        PhoneNumber phoneNumber, PhoneNumber? alternativePhone = null)
        : base()
    {
        FullName = fullName;
        Relationship = relationship;
        PhoneNumber = phoneNumber;
        AlternativePhone = alternativePhone;
    }

    public static EmergencyContact Create(string fullName, string relationship,
        PhoneNumber phoneNumber, PhoneNumber? alternativePhone = null)
    {
        if (string.IsNullOrWhiteSpace(fullName))
            throw new DomainException("Acil durum kişisi adı boş olamaz.");

        if (string.IsNullOrWhiteSpace(relationship))
            throw new DomainException("Yakınlık derecesi boş olamaz.");

        return new EmergencyContact(fullName.Trim(), relationship.Trim(),
            phoneNumber, alternativePhone);
    }

    public void Update(string fullName, string relationship,
        PhoneNumber phoneNumber, PhoneNumber? alternativePhone = null)
    {
        if (string.IsNullOrWhiteSpace(fullName))
            throw new DomainException("Acil durum kişisi adı boş olamaz.");

        if (string.IsNullOrWhiteSpace(relationship))
            throw new DomainException("Yakınlık derecesi boş olamaz.");

        FullName = fullName.Trim();
        Relationship = relationship.Trim();
        PhoneNumber = phoneNumber;
        AlternativePhone = alternativePhone;
    }
}