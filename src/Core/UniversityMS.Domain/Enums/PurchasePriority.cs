namespace UniversityMS.Domain.Enums;

/// <summary>
/// Satın Alma Önceliği - Mevcut (Tutuldu, 1→0)
/// </summary>
public enum PurchasePriority
{
    Low = 0,                 // Düşük
    Normal = 1,              // Normal
    High = 2,                // Yüksek
    Urgent = 3,              // Acil
    Critical = 4             // Kritik
}