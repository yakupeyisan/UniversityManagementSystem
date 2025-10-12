namespace UniversityMS.Domain.Enums;

/// <summary>
/// Oda Durumu
/// </summary>
public enum RoomStatus
{
    Available = 1,            // Müsait
    Occupied = 2,             // Dolu
    Reserved = 3,             // Rezerve
    UnderMaintenance = 4,     // Bakımda
    OutOfService = 5,         // Hizmet dışı
    Locked = 6                // Kilitli
}