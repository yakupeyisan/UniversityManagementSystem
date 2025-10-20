namespace UniversityMS.Domain.Enums;

/// <summary>
/// IT Talep Durumu
/// </summary>
public enum ITTicketStatus
{
    New = 1,
    Assigned = 2,
    InProgress = 3,
    OnHold = 4,
    Resolved = 5,
    Closed = 6,
    Reopened = 7
}