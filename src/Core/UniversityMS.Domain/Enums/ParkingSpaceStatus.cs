namespace UniversityMS.Domain.Enums;

/// <summary>
/// Park Yeri Durumu
/// </summary>
public enum ParkingSpaceStatus
{
    Available = 1,            // Müsait
    Occupied = 2,             // Dolu
    Reserved = 3,             // Rezerve
    OutOfService = 4          // Hizmet dışı
}