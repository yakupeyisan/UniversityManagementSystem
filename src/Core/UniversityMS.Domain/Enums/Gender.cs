namespace UniversityMS.Domain.Enums;
public enum Gender
{
    Male = 1,
    Female = 2,
    Other = 3,
    PreferNotToSay = 4
}
public enum StudentStatus
{
    Active = 1,
    Passive = 2,
    Suspended = 3,
    Graduated = 4,
    Withdrawn = 5,
    OnLeave = 6
}
public enum CourseType
{
    Compulsory = 1,    // Zorunlu
    Elective = 2,      // Seçmeli
    General = 3        // Genel
}
public enum GradeType
{
    Midterm = 1,       // Vize
    Final = 2,         // Final
    Makeup = 3,        // Bütünleme
    Quiz = 4,          // Quiz
    Assignment = 5,    // Ödev
    Project = 6,       // Proje
    Lab = 7            // Laboratuvar
}
public enum PaymentStatus
{
    Pending = 1,
    Completed = 2,
    Failed = 3,
    Cancelled = 4,
    Refunded = 5
}
public enum AcademicTitle
{
    Professor = 1,              // Profesör
    AssociateProfessor = 2,     // Doçent
    AssistantProfessor = 3,     // Dr. Öğretim Üyesi
    Lecturer = 4,               // Öğretim Görevlisi
    ResearchAssistant = 5,      // Araştırma Görevlisi
    Instructor = 6              // Okutman
}
public enum EducationLevel
{
    Associate = 1,     // Ön Lisans
    Bachelor = 2,      // Lisans
    Masters = 3,       // Yüksek Lisans
    Doctorate = 4      // Doktora
}

public enum EnrollmentStatus
{
    Draft = 1,        // Taslak
    Submitted = 2,    // Gönderildi
    Approved = 3,     // Onaylandı
    Rejected = 4,     // Reddedildi
    Cancelled = 5     // İptal
}

public enum CourseRegistrationStatus
{
    Active = 1,      // Aktif
    Dropped = 2,     // Bırakıldı
    Passed = 3,      // Geçti
    Failed = 4,      // Kaldı
    Incomplete = 5   // Tamamlanmadı
}

public enum AttendanceMethod
{
    Manual = 1,      // Manuel yoklama
    QRCode = 2,      // QR kod ile
    Card = 3,        // Kart okutma ile
    Biometric = 4    // Biyometrik
}