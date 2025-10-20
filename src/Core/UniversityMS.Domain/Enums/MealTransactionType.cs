namespace UniversityMS.Domain.Enums;

/// <summary>
/// Yemekhane İşlem Türü - NEW
/// </summary>
public enum MealTransactionType
{
    Recharge = 0,            // Yükleme
    Purchase = 1,            // Satın alma
    Refund = 2,              // İade
    Adjustment = 3,          // Düzeltme
    Fine = 4,                // Ceza
    Compensation = 5,        // Tazminat
    Bonus = 6                // Bonus
}