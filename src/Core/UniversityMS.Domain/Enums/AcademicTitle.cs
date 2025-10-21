namespace UniversityMS.Domain.Enums;

/// <summary>
/// Akademik Ünvan - NEW
/// </summary>
public enum AcademicTitle
{
    Professor = 0,           // Profesör
    AssociateProfessor = 1,  // Doçent
    AssistantProfessor = 2,  // Yardımcı Doçent
    Instructor = 3,          // Öğretim görevlisi
    Lecturer = 4,            // Okutman
    ResearchAssistant = 5,   // Araştırma görevlisi
    TeachingAssistant = 6,   // Öğretim asistanı
    PostDoc = 7              // Postdoc
}

/// <summary>
/// Erişim Şablonu Türü
/// Hangi tür kullanıcı grupları için şablon oluşturulacağını tanımlar
/// </summary>
public enum AccessTemplateType
{
    StudentUndergraduate = 0,        // Lisans Öğrencisi
    StudentGraduate = 1,             // Yüksek Lisans Öğrencisi
    StudentPhD = 2,                  // Doktora Öğrencisi
    InstructorAssistant = 3,         // Öğretim Görevlisi
    ResearchAssistant = 4,           // Araştırma Görevlisi
    Lecturer = 5,                    // Öğretim Üyesi
    Professor = 6,                   // Profesör
    Administrator = 7,               // İdari Personel
    SecurityPersonnel = 8,           // Güvenlik Görevlisi
    MaintenanceStaff = 9,            // Bakım Görevlisi
    Contractor = 10,                 // Yüklenici
    Visitor = 11,                    // Ziyaretçi
    CustomGroup = 12                 // Özel Grup
}