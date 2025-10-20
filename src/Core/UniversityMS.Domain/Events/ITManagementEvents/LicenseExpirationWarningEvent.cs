using UniversityMS.Domain.Interfaces;

namespace UniversityMS.Domain.Events.ITManagementEvents;

/// <summary>
/// Sistem lisansı süresi dolduktan yaklaşık olduğunda tetiklenen event
/// </summary>
public class LicenseExpirationWarningEvent : BaseDomainEvent
{
    public Guid LicenseId { get; }
    public string SoftwareName { get; }
    public DateTime ExpiryDate { get; }
    public int DaysRemaining { get; }

    public LicenseExpirationWarningEvent(Guid licenseId, string softwareName, DateTime expiryDate, int daysRemaining)
    {
        LicenseId = licenseId;
        SoftwareName = softwareName;
        ExpiryDate = expiryDate;
        DaysRemaining = daysRemaining;
    }
}