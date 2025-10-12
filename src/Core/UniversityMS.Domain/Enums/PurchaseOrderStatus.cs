namespace UniversityMS.Domain.Enums;

public enum PurchaseOrderStatus
{
    Draft = 1,
    Submitted = 2,
    Approved = 3,
    Rejected = 4,
    Sent = 5,
    PartiallyReceived = 6,
    Received = 7,
    Completed = 8,
    Cancelled = 9
}