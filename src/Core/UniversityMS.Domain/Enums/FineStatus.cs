namespace UniversityMS.Domain.Enums;

/// <summary>
/// Ceza Durumu - NEW
/// </summary>
public enum FineStatus
{
    Pending = 0,             // Beklemede
    Paid = 1,                // Ödendi
    Partial = 2,             // Kısmen ödendi
    Forgiven = 3,            // Affedildi
    WriteOff = 4,            // Terkedildi
    Waived = 5,              // ✅ Affedildi (Çalışmalı af) 
    Cancelled = 6            // ✅ İptal edildi 
}