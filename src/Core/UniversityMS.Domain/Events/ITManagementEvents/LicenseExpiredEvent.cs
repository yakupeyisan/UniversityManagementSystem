using UniversityMS.Domain.Interfaces;

namespace UniversityMS.Domain.Events.ITManagementEvents;

/// <summary>
/// Sistem lisansı süresi dolduğunda tetiklenen event
/// </summary>
public class LicenseExpiredEvent : BaseDomainEvent
{
    public Guid LicenseId { get; }
    public string SoftwareName { get; }
    public DateTime ExpiryDate { get; }

    public LicenseExpiredEvent(Guid licenseId, string softwareName, DateTime expiryDate)
    {
        LicenseId = licenseId;
        SoftwareName = softwareName;
        ExpiryDate = expiryDate;
    }
}