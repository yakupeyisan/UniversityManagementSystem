namespace UniversityMS.Domain.Enums;

/// <summary>
/// Olay Türü - NEW
/// </summary>
public enum IncidentType
{
    UnauthorizedAccess = 0,      // Yetkisiz erişim
    AccessDeviceFault = 1,       // Erişim cihazı arızası
    SuspiciousBehavior = 2,      // Şüpheli davranış
    SafetyHazard = 3,            // Güvenlik tehlikesi
    AlarmTriggered = 4,          // Alarm tetiklendi
    TrespassingAttempt = 5,      // İzinsiz giriş denemesi
    ViolenceOrThreat = 6,        // Şiddet veya tehdit
    TheftOrVandalism = 7,        // Hırsızlık veya vandalizm
    Other = 8                    // Diğer
}