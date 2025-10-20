using UniversityMS.Domain.Entities.Common;
using UniversityMS.Domain.Enums;

namespace UniversityMS.Domain.Entities.SecurityAggregate;

public class VisitorBadge : BaseEntity
{
    public Guid VisitorId { get; private set; }
    public string BadgeNumber { get; private set; } = null!;
    public BadgeStatus Status { get; private set; }
    public DateTime IssuedAt { get; private set; }
    public DateTime ExpiresAt { get; private set; }

    private VisitorBadge() { }

    public VisitorBadge(Guid visitorId, string badgeNumber, DateTime expiresAt)
    {
        VisitorId = visitorId;
        BadgeNumber = badgeNumber;
        Status = BadgeStatus.Active;
        IssuedAt = DateTime.UtcNow;
        ExpiresAt = expiresAt;
    }

    public void Revoke()
    {
        Status = BadgeStatus.Deactivated;
    }

    public bool IsExpired => DateTime.UtcNow > ExpiresAt;
}