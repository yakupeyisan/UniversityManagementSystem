using UniversityMS.Domain.Entities.Common;
using UniversityMS.Domain.Entities.PersonAggregate;
using UniversityMS.Domain.Enums;
using UniversityMS.Domain.Exceptions;
using UniversityMS.Domain.Interfaces;
using UniversityMS.Domain.ValueObjects;

namespace UniversityMS.Domain.Entities.AcademicAggregate;

/// <summary>
/// Kampüs (Campus) - Aggregate Root
/// Üniversitenin fiziksel kampüslerini temsil eder
/// </summary>
public class Campus : AuditableEntity, IAggregateRoot
{
    public string Code { get; private set; } = null!;
    public string Name { get; private set; } = null!;
    public CampusType Type { get; private set; }
    public CampusStatus Status { get; private set; }
    public Address Address { get; private set; } = null!;
    public decimal TotalArea { get; private set; } // m²
    public int? Capacity { get; private set; }
    public DateTime? EstablishedDate { get; private set; }
    public ContactInfo ContactInfo { get; private set; } = null!;
    public Guid? DirectorId { get; private set; }
    public string? Description { get; private set; }
    public bool IsMainCampus { get; private set; }
    public string? Website { get; private set; }

    private Campus() { }

    private Campus(
        string code,
        string name,
        CampusType type,
        Address address,
        ContactInfo contactInfo,
        decimal totalArea,
        bool isMainCampus = false)
    {
        Code = code;
        Name = name;
        Type = type;
        Address = address;
        ContactInfo = contactInfo;
        TotalArea = totalArea;
        IsMainCampus = isMainCampus;
        Status = CampusStatus.Active;
    }

    public static Campus Create(
        string code,
        string name,
        CampusType type,
        Address address,
        ContactInfo contactInfo,
        decimal totalArea,
        bool isMainCampus = false)
    {
        if (string.IsNullOrWhiteSpace(code))
            throw new DomainException("Kampüs kodu boş olamaz.");

        if (string.IsNullOrWhiteSpace(name))
            throw new DomainException("Kampüs adı boş olamaz.");

        if (totalArea <= 0)
            throw new DomainException("Toplam alan pozitif olmalıdır.");

        return new Campus(code, name, type, address, contactInfo, totalArea, isMainCampus);
    }

    public void Activate()
    {
        Status = CampusStatus.Active;
    }

    public void Deactivate()
    {
        Status = CampusStatus.Inactive;
    }

    public void SetUnderConstruction()
    {
        Status = CampusStatus.UnderConstruction;
    }

    public void SetUnderRenovation()
    {
        Status = CampusStatus.UnderRenovation;
    }

    public void SetDirector(Guid directorId)
    {
        DirectorId = directorId;
    }

    public void UpdateCapacity(int capacity)
    {
        if (capacity < 0)
            throw new DomainException("Kapasite negatif olamaz.");

        Capacity = capacity;
    }

    public void UpdateContactInfo(ContactInfo contactInfo)
    {
        ContactInfo = contactInfo ?? throw new ArgumentNullException(nameof(contactInfo));
    }

    public bool IsOperational()
    {
        return Status == CampusStatus.Active;
    }
}