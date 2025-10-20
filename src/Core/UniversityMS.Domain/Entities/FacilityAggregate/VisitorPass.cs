using UniversityMS.Domain.Entities.Common;
using UniversityMS.Domain.Enums;
using UniversityMS.Domain.Exceptions;

namespace UniversityMS.Domain.Entities.FacilityAggregate;

public class VisitorPass : AuditableEntity
{
    public Guid VisitorId { get; private set; }
    public string PassNumber { get; private set; } = string.Empty;
    public DateTime IssuedDate { get; private set; }
    public DateTime ExpiryDate { get; private set; }
    public VisitorPassStatus Status { get; private set; }
    public string? AccessLevel { get; private set; }
    public List<string> AllowedAreas { get; private set; } = new();

    private VisitorPass() { }

    private VisitorPass(Guid visitorId, string passNumber, DateTime expiryDate)
    {
        VisitorId = visitorId;
        PassNumber = passNumber;
        IssuedDate = DateTime.UtcNow;
        ExpiryDate = expiryDate;
        Status = VisitorPassStatus.Active;
    }

    public static VisitorPass Create(Guid visitorId, string passNumber, int validDays = 1)
    {
        if (visitorId == Guid.Empty)
            throw new DomainException("Ziyaretçi ID boş olamaz.");
        if (string.IsNullOrWhiteSpace(passNumber))
            throw new DomainException("Kart numarası boş olamaz.");

        return new VisitorPass(visitorId, passNumber.Trim(), DateTime.UtcNow.AddDays(validDays));
    }

    public void Deactivate() => Status = VisitorPassStatus.Inactive;
    public void SetAccessLevel(string level) => AccessLevel = level;
    public void AddAllowedArea(string area) => AllowedAreas.Add(area);
}