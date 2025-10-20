namespace UniversityMS.Domain.Enums;

/// <summary>Erişim Durumu</summary>
public enum AccessStatus
{
    Allowed = 0,
    Denied = 1,
    Pending = 2,
    Suspicious = 3,
    ManualApprovalNeeded = 4
}