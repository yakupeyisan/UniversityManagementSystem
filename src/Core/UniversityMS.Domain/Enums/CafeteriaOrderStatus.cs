namespace UniversityMS.Domain.Enums;

/// <summary>
/// Yemekhane Siparişi Durumu
/// </summary>
public enum CafeteriaOrderStatus
{
    Pending = 1,
    Approved = 2,
    Preparing = 3,
    Ready = 4,
    Delivered = 5,
    Cancelled = 6
}