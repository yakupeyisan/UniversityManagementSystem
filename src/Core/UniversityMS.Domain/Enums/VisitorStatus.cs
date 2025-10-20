namespace UniversityMS.Domain.Enums;


/// <summary>
/// Ziyaretçi Durumu - NEW
/// </summary>
public enum VisitorStatus
{
    CheckedIn = 0,           // Giriş yaptı
    CheckedOut = 1,          // Çıkış yaptı
    NoShow = 2,              // Gelmedi
    EarlyCheckOut = 3,       // Erken çıkış
    Banned = 4,              // Yasaklı
    Suspended = 5            // Askıya alındı
}