namespace UniversityMS.Domain.Enums;

/// <summary>
/// Oda Tipi - Mevcut (Tutuldu)
/// </summary>
public enum RoomType
{
    Classroom = 0,           // Sınıf
    LectureHall = 1,         // Amfi
    Laboratory = 2,          // Laboratuvar
    Office = 3,              // Ofis
    MeetingRoom = 4,         // Toplantı
    ConferenceRoom = 5,      // Konferans
    Library = 6,             // Kütüphane
    StudyRoom = 7,           // Çalışma
    ComputerLab = 8,         // Bilgisayar lab
    Studio = 9,              // Stüdyo
    Workshop = 10,           // Atölye
    Storage = 11,            // Depo
    Cafeteria = 12,          // Kafeterya
    Restroom = 13,           // Tuvalet
    Lounge = 14,             // Dinlenme
    ServerRoom = 15,         // Sunucu
    SecurityRoom = 16,       // Güvenlik
    HealthRoom = 17,         // Sağlık
    Other = 99               // Diğer
}