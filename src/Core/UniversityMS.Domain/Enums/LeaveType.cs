namespace UniversityMS.Domain.Enums;

/// <summary>
/// İzin Türü - NEW
/// </summary>
public enum LeaveType
{
    Annual = 0,              // Yıllık
    Sick = 1,                // Hastalık
    Maternity = 2,           // Analık
    Paternity = 3,           // Babalık
    Bereavement = 4,         // Ölüm
    Unpaid = 5,              // Ücretsiz
    Sabbatical = 6,          // Sabatik
    Special = 7,              // Özel
    Compensatory = 8         // ✅ Telafi (Fazla çalışmanın telafisi) 
}