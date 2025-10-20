using UniversityMS.Domain.Entities.Common;
using UniversityMS.Domain.Entities.PersonAggregate;
using UniversityMS.Domain.Enums;
using UniversityMS.Domain.Events.SecurityEvents;
using UniversityMS.Domain.Exceptions;
using UniversityMS.Domain.Interfaces;

namespace UniversityMS.Domain.Entities.SecurityAggregate;

/// <summary>
/// Ziyaretçi (Visitor) - Aggregate Root (YENİ)
/// Kampüs ziyaretçilerini ve badge'lerini yönetir
/// </summary>
public class Visitor : AuditableEntity, IAggregateRoot
{
    public string VisitorNumber { get; private set; } = null!;
    public string FirstName { get; private set; } = null!;
    public string LastName { get; private set; } = null!;
    public string Email { get; private set; } = null!;
    public string PhoneNumber { get; private set; } = null!;
    public string CompanyName { get; private set; } = null!;
    public Guid? CampusId { get; private set; }
    public Guid? HostUserId { get; private set; }
    public DateTime CheckInTime { get; private set; }
    public DateTime? CheckOutTime { get; private set; }
    public VisitorStatus Status { get; private set; }
    public string? IDNumber { get; private set; }
    public string? IDType { get; private set; }
    public string? Purpose { get; private set; }
    public string? Notes { get; private set; }

    private readonly List<VisitorBadge> _badges = new();
    public IReadOnlyCollection<VisitorBadge> Badges => _badges.AsReadOnly();

    private Visitor() { }

    public static Visitor Create(
        string firstName,
        string lastName,
        string email,
        string phoneNumber,
        string companyName,
        Guid? campusId = null,
        Guid? hostUserId = null,
        string? purpose = null)
    {
        if (string.IsNullOrWhiteSpace(firstName))
            throw new DomainException("Adı boş olamaz.");
        if (string.IsNullOrWhiteSpace(lastName))
            throw new DomainException("Soyadı boş olamaz.");
        if (string.IsNullOrWhiteSpace(email))
            throw new DomainException("Email boş olamaz.");

        var visitor = new Visitor
        {
            FirstName = firstName,
            LastName = lastName,
            Email = email,
            PhoneNumber = phoneNumber,
            CompanyName = companyName,
            CampusId = campusId,
            HostUserId = hostUserId,
            Purpose = purpose,
            CheckInTime = DateTime.UtcNow,
            Status = VisitorStatus.CheckedIn
        };

        visitor.GenerateVisitorNumber();
        visitor.AddDomainEvent(new VisitorCheckInEvent(visitor.Id, campusId));
        return visitor;
    }

    private void GenerateVisitorNumber()
    {
        VisitorNumber = $"VIS{DateTime.Now:yyyyMMddHHmmss}{Random.Shared.Next(1000, 9999)}";
    }

    public void CheckOut()
    {
        if (Status != VisitorStatus.CheckedIn)
            throw new DomainException("Ziyaretçi zaten işlem görmüştür.");

        CheckOutTime = DateTime.UtcNow;
        Status = VisitorStatus.CheckedOut;
        AddDomainEvent(new VisitorCheckOutEvent(Id, CampusId));
    }

    public void AddBadge(VisitorBadge badge)
    {
        if (badge.VisitorId != Id)
            throw new DomainException("Badge bu ziyaretçiye ait değil.");
        _badges.Add(badge);
    }

    public void IssueBadge(DateTime expiresAt)
    {
        var badge = new VisitorBadge(Id, GenerateBadgeNumber(), expiresAt);
        AddBadge(badge);
        AddDomainEvent(new VisitorBadgeIssuedEvent(Id, badge.BadgeNumber));
    }

    private string GenerateBadgeNumber()
    {
        return $"VB{DateTime.Now:yyyyMMddHHmmss}{Random.Shared.Next(1000, 9999)}";
    }
}