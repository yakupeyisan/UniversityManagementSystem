namespace UniversityMS.Domain.Enums;

/// <summary>
/// Acil Durum Alarm Durumu
/// </summary>
public enum EmergencyAlertStatus
{
    Active = 1,               // Aktif
    Acknowledged = 2,         // Onaylandı
    Responding = 3,           // Müdahale ediliyor
    Resolved = 4,             // Çözüldü
    FalseAlarm = 5,           // Yanlış alarm
    Cancelled = 6             // İptal edildi
}