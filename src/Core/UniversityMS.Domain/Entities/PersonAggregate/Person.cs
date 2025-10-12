using System.Net;
using UniversityMS.Domain.Entities.Common;
using UniversityMS.Domain.Enums;
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

    public void Delete(string? deletedBy = null)
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