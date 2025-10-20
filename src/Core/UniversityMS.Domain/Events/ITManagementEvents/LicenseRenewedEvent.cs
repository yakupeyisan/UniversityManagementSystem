using UniversityMS.Domain.Interfaces;

namespace UniversityMS.Domain.Events.ITManagementEvents;

/// <summary>
/// Sistem lisansı yenilendiğinde tetiklenen event
/// </summary>
public class LicenseRenewedEvent : BaseDomainEvent
{
    public Guid LicenseId { get; }
    public string SoftwareName { get; }
    public DateTime NewExpiryDate { get; }

    public LicenseRenewedEvent(Guid licenseId, string softwareName, DateTime newExpiryDate)
    {
        LicenseId = licenseId;
        SoftwareName = softwareName;
        NewExpiryDate = newExpiryDate;
    }
}