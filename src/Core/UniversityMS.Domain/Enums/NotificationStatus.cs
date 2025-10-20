namespace UniversityMS.Domain.Enums;

/// <summary>
/// Bildirim Durumu
/// </summary>
public enum NotificationStatus
{
    Created = 1,
    Queued = 2,
    Sent = 3,
    Delivered = 4,
    Read = 5,
    Failed = 6,
    Archived = 7
}