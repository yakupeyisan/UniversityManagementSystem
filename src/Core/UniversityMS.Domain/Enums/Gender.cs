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
    OnLeave = 6,
    Frozen = 7,
    Graduate = 8
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

public enum ObjectionStatus
{
    Pending = 0,        // Beklemede
    UnderReview = 1,    // İnceleme altında
    Approved = 2,       // Onaylandı
    Rejected = 3        // Reddedildi
}


/// <summary>
/// Çalışan istihdam durumu
/// </summary>
public enum EmploymentStatus
{
    /// <summary>
    /// Aktif - Çalışıyor
    /// </summary>
    Active = 1,

    /// <summary>
    /// İzinli
    /// </summary>
    OnLeave = 2,

    /// <summary>
    /// Geçici olarak işten çıkarılmış (ücretsiz izin, vb.)
    /// </summary>
    Suspended = 3,

    /// <summary>
    /// İstifa etti
    /// </summary>
    Resigned = 4,

    /// <summary>
    /// İşten çıkarıldı
    /// </summary>
    Terminated = 5,

    /// <summary>
    /// Emekli
    /// </summary>
    Retired = 6,

    /// <summary>
    /// Vefat etti
    /// </summary>
    Deceased = 7
}

/// <summary>
/// Sözleşme türü
/// </summary>
public enum ContractType
{
    /// <summary>
    /// Belirsiz süreli iş sözleşmesi
    /// </summary>
    Permanent = 1,

    /// <summary>
    /// Belirli süreli iş sözleşmesi
    /// </summary>
    FixedTerm = 2,

    /// <summary>
    /// Part-time / Yarı zamanlı
    /// </summary>
    PartTime = 3,

    /// <summary>
    /// Proje bazlı
    /// </summary>
    ProjectBased = 4,

    /// <summary>
    /// Danışman / Konsültan
    /// </summary>
    Consultant = 5,

    /// <summary>
    /// Stajyer
    /// </summary>
    Intern = 6
}

/// <summary>
/// Sözleşme durumu
/// </summary>
public enum ContractStatus
{
    /// <summary>
    /// Taslak - Henüz onaylanmadı
    /// </summary>
    Draft = 1,

    /// <summary>
    /// Aktif - Yürürlükte
    /// </summary>
    Active = 2,

    /// <summary>
    /// Yenileme bekliyor
    /// </summary>
    PendingRenewal = 3,

    /// <summary>
    /// Yenilendi
    /// </summary>
    Renewed = 4,

    /// <summary>
    /// Sona erdi
    /// </summary>
    Expired = 5,

    /// <summary>
    /// Feshedildi
    /// </summary>
    Terminated = 6,

    /// <summary>
    /// İptal edildi
    /// </summary>
    Cancelled = 7
}

/// <summary>
/// İzin türü
/// </summary>
public enum LeaveType
{
    /// <summary>
    /// Yıllık izin
    /// </summary>
    Annual = 1,

    /// <summary>
    /// Hastalık izni (raporlu)
    /// </summary>
    Sick = 2,

    /// <summary>
    /// Mazeret izni
    /// </summary>
    Excuse = 3,

    /// <summary>
    /// Doğum izni (ebeveyn)
    /// </summary>
    Maternity = 4,

    /// <summary>
    /// Babalık izni
    /// </summary>
    Paternity = 5,

    /// <summary>
    /// Evlilik izni
    /// </summary>
    Marriage = 6,

    /// <summary>
    /// Ölüm izni (vefat)
    /// </summary>
    Bereavement = 7,

    /// <summary>
    /// Ücretsiz izin
    /// </summary>
    Unpaid = 8,

    /// <summary>
    /// Askeri izin
    /// </summary>
    Military = 9,

    /// <summary>
    /// Eğitim izni
    /// </summary>
    Study = 10,

    /// <summary>
    /// Saatlik izin
    /// </summary>
    Hourly = 11
}

/// <summary>
/// İzin durumu
/// </summary>
public enum LeaveStatus
{
    /// <summary>
    /// Talep edildi - Onay bekliyor
    /// </summary>
    Pending = 1,

    /// <summary>
    /// Onaylandı
    /// </summary>
    Approved = 2,

    /// <summary>
    /// Reddedildi
    /// </summary>
    Rejected = 3,

    /// <summary>
    /// İptal edildi (çalışan tarafından)
    /// </summary>
    Cancelled = 4,

    /// <summary>
    /// Tamamlandı - İzin kullanıldı
    /// </summary>
    Completed = 5
}

/// <summary>
/// Vardiya deseni
/// </summary>
public enum ShiftPattern
{
    /// <summary>
    /// Gündüz vardiyası (08:00-17:00)
    /// </summary>
    Day = 1,

    /// <summary>
    /// Akşam vardiyası (16:00-00:00)
    /// </summary>
    Evening = 2,

    /// <summary>
    /// Gece vardiyası (00:00-08:00)
    /// </summary>
    Night = 3,

    /// <summary>
    /// Esnek çalışma
    /// </summary>
    Flexible = 4,

    /// <summary>
    /// Rotasyonlu (dönüşümlü)
    /// </summary>
    Rotating = 5,

    /// <summary>
    /// Hafta sonu
    /// </summary>
    Weekend = 6
}

/// <summary>
/// Vardiya durumu
/// </summary>
public enum ShiftStatus
{
    /// <summary>
    /// Planlandı
    /// </summary>
    Scheduled = 1,

    /// <summary>
    /// Devam ediyor
    /// </summary>
    InProgress = 2,

    /// <summary>
    /// Tamamlandı
    /// </summary>
    Completed = 3,

    /// <summary>
    /// İptal edildi
    /// </summary>
    Cancelled = 4,

    /// <summary>
    /// Değiştirildi
    /// </summary>
    Modified = 5
}

/// <summary>
/// Performans değerlendirme durumu
/// </summary>
public enum PerformanceReviewStatus
{
    /// <summary>
    /// Planlandı
    /// </summary>
    Scheduled = 1,

    /// <summary>
    /// Devam ediyor
    /// </summary>
    InProgress = 2,

    /// <summary>
    /// Tamamlandı
    /// </summary>
    Completed = 3,

    /// <summary>
    /// Onay bekliyor
    /// </summary>
    PendingApproval = 4,

    /// <summary>
    /// Onaylandı
    /// </summary>
    Approved = 5,

    /// <summary>
    /// İptal edildi
    /// </summary>
    Cancelled = 6
}

/// <summary>
/// Performans derecesi
/// </summary>
public enum PerformanceRating
{
    /// <summary>
    /// Beklentilerin altında
    /// </summary>
    BelowExpectations = 1,

    /// <summary>
    /// Geliştirilmeli
    /// </summary>
    NeedsImprovement = 2,

    /// <summary>
    /// Beklentileri karşılıyor
    /// </summary>
    MeetsExpectations = 3,

    /// <summary>
    /// Beklentilerin üzerinde
    /// </summary>
    ExceedsExpectations = 4,

    /// <summary>
    /// Mükemmel
    /// </summary>
    Outstanding = 5
}

/// <summary>
/// Eğitim durumu
/// </summary>
public enum TrainingStatus
{
    /// <summary>
    /// Planlandı
    /// </summary>
    Planned = 1,

    /// <summary>
    /// Kayıt açık
    /// </summary>
    RegistrationOpen = 2,

    /// <summary>
    /// Devam ediyor
    /// </summary>
    InProgress = 3,

    /// <summary>
    /// Tamamlandı
    /// </summary>
    Completed = 4,

    /// <summary>
    /// İptal edildi
    /// </summary>
    Cancelled = 5,

    /// <summary>
    /// Ertelendi
    /// </summary>
    Postponed = 6
}

/// <summary>
/// Eğitim türü
/// </summary>
public enum TrainingType
{
    /// <summary>
    /// Teknik eğitim
    /// </summary>
    Technical = 1,

    /// <summary>
    /// Yönetsel eğitim
    /// </summary>
    Managerial = 2,

    /// <summary>
    /// Kişisel gelişim
    /// </summary>
    PersonalDevelopment = 3,

    /// <summary>
    /// Uyum eğitimi (onboarding)
    /// </summary>
    Onboarding = 4,

    /// <summary>
    /// İş sağlığı ve güvenliği
    /// </summary>
    HealthAndSafety = 5,

    /// <summary>
    /// Sertifikasyon
    /// </summary>
    Certification = 6,

    /// <summary>
    /// Konferans / Seminer
    /// </summary>
    Conference = 7,

    /// <summary>
    /// Workshop
    /// </summary>
    Workshop = 8
}