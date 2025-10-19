namespace UniversityMS.Domain.Enums;
public enum LeaveType
{
    Annual = 1,              // Yıllık izin
    Sick = 2,                // Hastalık izni
    Maternity = 3,           // Doğum izni
    Paternity = 4,           // Babalık izni
    Marriage = 5,            // Evlilik izni
    Death = 6,               // Ölüm izni
    Unpaid = 7,              // Ücretsiz izin
    Study = 8,               // Eğitim izni
    Military = 9,            // Askerlik izni
    Pilgrimage = 10,         // Hac izni
    Exam = 11,               // Sınav izni
    Compensatory = 12,       // Telafi izni (Fazla mesai karşılığı)
    Other = 99
}
