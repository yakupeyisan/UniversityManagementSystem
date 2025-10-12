using UniversityMS.Domain.Entities.Common;
using UniversityMS.Domain.Entities.PersonAggregate;
using UniversityMS.Domain.Enums;
using UniversityMS.Domain.Events.SecurityEvents;
using UniversityMS.Domain.Exceptions;

namespace UniversityMS.Domain.Entities.SecurityAggregate;

/// <summary>
/// Ziyaretçi (Visitor) Entity
/// </summary>
public class Visitor : AuditableEntity
{
    public string FullName { get; private set; } = null!;
    public string? Company { get; private set; }
    public string? PhoneNumber { get; private set; }
    public string? Email { get; private set; }
    public string? IdentificationNumber { get; private set; }
    public VisitorType Type { get; private set; }
    public VisitorStatus Status { get; private set; }
    public Guid HostId { get; private set; }
    public Guid? DepartmentId { get; private set; }
    public DateTime? ExpectedArrival { get; private set; }
    public DateTime? CheckInTime { get; private set; }
    public DateTime? CheckOutTime { get; private set; }
    public DateTime? ValidUntil { get; private set; }
    public string? BadgeNumber { get; private set; }
    public string? PurposeOfVisit { get; private set; }
    public string? Notes { get; private set; }
    public string? PhotoUrl { get; private set; }
    public bool RequiresEscort { get; private set; }
    public string? EscortedBy { get; private set; }

    // Navigation Properties
    public Person Host { get; private set; } = null!;

    private Visitor() { }

    private Visitor(
        string fullName,
        VisitorType type,
        Guid hostId,
        string? purposeOfVisit = null,
        DateTime? expectedArrival = null,
        bool requiresEscort = false)
    {
        FullName = fullName;
        Type = type;
        HostId = hostId;
        PurposeOfVisit = purposeOfVisit;
        ExpectedArrival = expectedArrival;
        RequiresEscort = requiresEscort;
        Status = expectedArrival.HasValue ? VisitorStatus.PreRegistered : VisitorStatus.CheckedIn;
    }

    public static Visitor Create(
        string fullName,
        VisitorType type,
        Guid hostId,
        string? purposeOfVisit = null,
        DateTime? expectedArrival = null,
        bool requiresEscort = false)
    {
        if (string.IsNullOrWhiteSpace(fullName))
            throw new DomainException("Ziyaretçi adı boş olamaz.");

        return new Visitor(fullName, type, hostId, purposeOfVisit, expectedArrival, requiresEscort);
    }

    public void UpdateContactInfo(string? phoneNumber, string? email, string? company = null)
    {
        PhoneNumber = phoneNumber;
        Email = email;
        Company = company;
    }

    public void CheckIn(string? badgeNumber = null, int validityHours = 8)
    {
        if (Status == VisitorStatus.CheckedIn || Status == VisitorStatus.InBuilding)
            throw new DomainException("Ziyaretçi zaten giriş yapmış.");

        if (Status == VisitorStatus.Denied || Status == VisitorStatus.Blacklisted)
            throw new DomainException("Ziyaretçinin girişi engellenmiş.");

        CheckInTime = DateTime.UtcNow;
        ValidUntil = DateTime.UtcNow.AddHours(validityHours);
        Status = VisitorStatus.CheckedIn;
        BadgeNumber = badgeNumber;

        AddDomainEvent(new VisitorCheckedInEvent(Id, FullName, HostId, CheckInTime.Value));
    }

    public void CheckOut()
    {
        if (Status != VisitorStatus.CheckedIn && Status != VisitorStatus.InBuilding)
            throw new DomainException("Ziyaretçi giriş yapmamış.");

        CheckOutTime = DateTime.UtcNow;
        Status = VisitorStatus.CheckedOut;

        AddDomainEvent(new VisitorCheckedOutEvent(Id, FullName, CheckOutTime.Value));
    }

    public void Deny(string reason)
    {
        Status = VisitorStatus.Denied;
        Notes = $"Giriş reddedildi: {reason}";
    }

    public void Blacklist(string reason)
    {
        Status = VisitorStatus.Blacklisted;
        Notes = $"Kara listeye alındı: {reason}";
    }

    public void AssignEscort(string escortName)
    {
        if (!RequiresEscort)
            throw new DomainException("Bu ziyaretçi için refakat gerekmiyor.");

        EscortedBy = escortName;
    }

    public void SetIdentification(string idNumber, string? photoUrl = null)
    {
        IdentificationNumber = idNumber;
        PhotoUrl = photoUrl;
    }

    public void ExtendValidity(int additionalHours)
    {
        if (!ValidUntil.HasValue)
            throw new DomainException("Geçerlilik süresi belirlenmemiş.");

        ValidUntil = ValidUntil.Value.AddHours(additionalHours);
    }

    public void CheckExpiry()
    {
        if (Status == VisitorStatus.CheckedIn &&
            ValidUntil.HasValue &&
            DateTime.UtcNow > ValidUntil.Value)
        {
            Status = VisitorStatus.Expired;
        }
    }

    public bool IsExpired()
    {
        return ValidUntil.HasValue && DateTime.UtcNow > ValidUntil.Value;
    }

    public bool IsInBuilding()
    {
        return Status == VisitorStatus.CheckedIn || Status == VisitorStatus.InBuilding;
    }

    public TimeSpan? GetVisitDuration()
    {
        if (!CheckInTime.HasValue)
            return null;

        var endTime = CheckOutTime ?? DateTime.UtcNow;
        return endTime - CheckInTime.Value;
    }
}