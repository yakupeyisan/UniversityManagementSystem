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


public enum EmploymentStatus
{
    Active = 1,
    OnLeave = 2,
    Suspended = 3,
    Terminated = 4,
    Resigned = 5,
    Retired = 6
}

public enum ContractType
{
    Permanent = 1,        // Süresiz
    FixedTerm = 2,        // Belirli süreli
    PartTime = 3,         // Yarı zamanlı
    Temporary = 4,        // Geçici
    Internship = 5,       // Stajyer
    Consultant = 6        // Danışman
}

public enum ContractStatus
{
    Draft = 1,
    Active = 2,
    Expired = 3,
    Terminated = 4,
    Renewed = 5,
    PendingRenewal = 6,
    Cancelled = 7
}

public enum LeaveType
{
    Annual = 1,           // Yıllık izin
    Sick = 2,             // Hastalık izni
    Maternity = 3,        // Doğum izni
    Paternity = 4,        // Babalık izni
    Marriage = 5,         // Evlilik izni
    Death = 6,            // Ölüm izni
    Unpaid = 7,           // Ücretsiz izin
    Study = 8,            // Eğitim izni
    Military = 9,         // Askerlik izni
    Pilgrimage = 10,      // Hac izni
    Exam = 11,            // Sınav izni
    Other = 99
}

public enum LeaveStatus
{
    Pending = 1,
    Approved = 2,
    Rejected = 3,
    Cancelled = 4,
    Completed = 5
}

public enum ShiftPattern
{
    Morning = 1,          // 08:00-16:00
    Afternoon = 2,        // 16:00-00:00
    Night = 3,            // 00:00-08:00
    Day = 4,              // 08:00-20:00
    Flexible = 5,         // Esnek
    Custom = 99           // Özel
}

public enum ShiftStatus
{
    Scheduled = 1,
    InProgress = 2,
    Completed = 3,
    Cancelled = 4,
    Modified = 5,
    NoShow = 6
}

public enum PerformanceReviewStatus
{
    Scheduled = 1,
    InProgress = 2,
    Completed = 3,
    Approved = 4,
    Cancelled = 5,
    PendingApproval = 6
}


public enum TrainingType
{
    Orientation = 1,      // Oryantasyon
    Technical = 2,        // Teknik
    Soft = 3,             // Soft skill
    Safety = 4,           // İş güvenliği
    Compliance = 5,       // Uyumluluk
    Leadership = 6,       // Liderlik
    Language = 7,         // Dil
    Certificate = 8,      // Sertifika programı
    Online = 9,           // Online
    Workshop = 10,        // Workshop
    Seminar = 11,         // Seminer
    Conference = 12       // Konferans
}

public enum TrainingStatus
{
    Planned = 1,
    Registration = 2,     // Kayıt açık
    InProgress = 3,
    Completed = 4,
    Cancelled = 5,
    Postponed = 6,
    RegistrationOpen = 7
}

public enum TrainingEnrollmentStatus
{
    Enrolled = 1,
    InProgress = 2,
    Completed = 3,
    Failed = 4,
    Cancelled = 5,
    NoShow = 6
}


public enum PayrollStatus
{
    Draft = 1,            // Taslak
    Calculated = 2,       // Hesaplanmış
    Approved = 3,         // Onaylanmış
    Rejected = 4,         // Reddedilmiş
    Paid = 5,             // Ödenmiş
    Cancelled = 6         // İptal
}

public enum PayrollItemType
{
    Earning = 1,          // Kazanç (ekler)
    Deduction = 2         // Kesinti
}

public enum DeductionType
{
    Tax = 1,              // Gelir Vergisi
    SocialSecurity = 2,   // SGK
    StampDuty = 3,        // Damga Vergisi
    UnemploymentInsurance = 4,  // İşsizlik Sigortası
    UnionDues = 5,        // Sendika Aidatı
    Loan = 6,             // Avans/Kredi kesintisi
    Garnishment = 7,      // Haciz/İcra
    Insurance = 8,        // Sigorta
    Pension = 9,          // Emekli sandığı
    Other = 99            // Diğer
}

public enum PaymentMethod
{
    Cash = 1,             // Nakit
    BankTransfer = 2,     // Havale/EFT
    Check = 3,            // Çek
    DirectDeposit = 4     // Otomatik ödeme
}

public enum BudgetType
{
    Operational = 1,      // İşletme bütçesi
    Capital = 2,          // Yatırım bütçesi
    Research = 3,         // Ar-Ge bütçesi
    Project = 4,          // Proje bütçesi
    Department = 5,       // Birim bütçesi
    Special = 6           // Özel bütçe
}

public enum BudgetStatus
{
    Draft = 1,            // Taslak
    UnderReview = 2,      // İncelemede
    Approved = 3,         // Onaylanmış
    Rejected = 4,         // Reddedilmiş
    Active = 5,           // Aktif
    Closed = 6,           // Kapalı
    Revised = 7           // Revize edilmiş
}

public enum BudgetCategory
{
    Personnel = 1,        // Personel giderleri
    Supplies = 2,         // Sarf malzeme
    Equipment = 3,        // Ekipman
    Services = 4,         // Hizmet alımları
    Travel = 5,           // Seyahat
    Utilities = 6,        // Faturalar (elektrik, su vs.)
    Maintenance = 7,      // Bakım-onarım
    Marketing = 8,        // Pazarlama
    IT = 9,               // Bilgi teknolojileri
    Training = 10,        // Eğitim
    Research = 11,        // Araştırma
    Construction = 12,    // İnşaat
    Other = 99            // Diğer
}

public enum TransactionType
{
    Income = 1,           // Gelir
    Expense = 2,          // Gider
    Transfer = 3,         // Transfer
    Adjustment = 4        // Düzeltme
}

public enum TransactionStatus
{
    Pending = 1,          // Beklemede
    Completed = 2,        // Tamamlandı
    Failed = 3,           // Başarısız
    Cancelled = 4,        // İptal
    Reversed = 5          // İade/Ters kayıt
}

public enum InvoiceType
{
    Sales = 1,            // Satış faturası
    Purchase = 2,         // Alış faturası
    Service = 3,          // Hizmet faturası
    CreditNote = 4,       // İade faturası
    Proforma = 5,         // Proforma fatura
    Export = 6,           // İhracat faturası
    Import = 7            // İthalat faturası
}

public enum InvoiceStatus
{
    Draft = 1,            // Taslak
    Issued = 2,           // Kesilmiş
    Sent = 3,             // Gönderilmiş
    Paid = 4,             // Ödendi
    PartiallyPaid = 5,    // Kısmi ödendi
    Overdue = 6,          // Vadesi geçmiş
    Cancelled = 7,        // İptal
    Refunded = 8          // İade edildi
}

public enum AccountType
{
    Asset = 1,            // Aktif
    Liability = 2,        // Pasif
    Equity = 3,           // Özsermaye
    Revenue = 4,          // Gelir
    Expense = 5,          // Gider
    Contra = 6            // Kontra hesap
}

public enum JournalEntryStatus
{
    Draft = 1,            // Taslak
    Posted = 2,           // Kaydedilmiş
    Approved = 3,         // Onaylanmış
    Reversed = 4,         // Ters kayıt
    Cancelled = 5         // İptal
}

// Satın Alma Modülü için Enumlar

public enum PurchaseRequestStatus
{
    Draft = 1,
    Submitted = 2,
    UnderReview = 3,
    Approved = 4,
    Rejected = 5,
    Cancelled = 6,
    Completed = 7
}

public enum PurchasePriority
{
    Low = 1,
    Normal = 2,
    High = 3,
    Urgent = 4,
    Critical = 5
}

public enum PurchaseOrderStatus
{
    Draft = 1,
    Submitted = 2,
    Approved = 3,
    Rejected = 4,
    Sent = 5,
    PartiallyReceived = 6,
    Received = 7,
    Completed = 8,
    Cancelled = 9
}

public enum SupplierType
{
    Manufacturer = 1,     // Üretici
    Distributor = 2,      // Distribütör
    Wholesaler = 3,       // Toptancı
    Retailer = 4,         // Perakendeci
    Service = 5,          // Hizmet sağlayıcı
    Contractor = 6,       // Yüklenici
    Consultant = 7        // Danışman
}

public enum SupplierStatus
{
    Active = 1,
    Inactive = 2,
    Suspended = 3,
    Blacklisted = 4,
    UnderReview = 5
}

public enum TenderType
{
    OpenTender = 1,           // Açık ihale
    RestrictedTender = 2,     // Belli istekliler arası ihale
    Negotiation = 3,          // Pazarlık usulü
    DirectProcurement = 4,    // Doğrudan temin
    TwoStage = 5,             // İki aşamalı ihale
    Framework = 6             // Çerçeve anlaşma
}

public enum TenderStatus
{
    Planning = 1,
    Published = 2,
    BidSubmission = 3,
    Evaluation = 4,
    Awarded = 5,
    Rejected = 6,
    Cancelled = 7,
    Completed = 8
}

// Stok Modülü için Enumlar

public enum WarehouseType
{
    Main = 1,             // Ana depo
    Department = 2,       // Birim deposu
    Laboratory = 3,       // Laboratuvar deposu
    IT = 4,               // IT deposu
    Maintenance = 5,      // Bakım deposu
    Archive = 6,          // Arşiv
    Cold = 7,             // Soğuk depo
    Hazardous = 8         // Tehlikeli madde deposu
}

public enum WarehouseStatus
{
    Active = 1,
    Inactive = 2,
    Maintenance = 3,
    Full = 4,
    Closed = 5
}

public enum StockCategory
{
    OfficeSuplies = 1,        // Ofis malzemeleri
    Cleaning = 2,             // Temizlik malzemeleri
    Laboratory = 3,           // Laboratuvar malzemeleri
    IT = 4,                   // Bilgisayar ve elektronik
    Furniture = 5,            // Mobilya
    Equipment = 6,            // Teknik ekipman
    Stationery = 7,           // Kırtasiye
    Consumables = 8,          // Sarf malzemeler
    ChemicalReagents = 9,     // Kimyasal reaktifler
    MedicalSupplies = 10,     // Tıbbi malzemeler
    Food = 11,                // Gıda maddeleri
    MaintenanceTools = 12,    // Bakım araçları
    SafetyEquipment = 13,     // Güvenlik ekipmanları
    Other = 99
}

public enum StockMovementType
{
    In = 1,               // Giriş
    Out = 2,              // Çıkış
    Transfer = 3,         // Transfer
    Adjustment = 4,       // Düzeltme
    Return = 5,           // İade
    Damaged = 6,          // Hasarlı
    Expired = 7,          // Vadesi geçmiş
    Lost = 8              // Kayıp
}

public enum StockCountStatus
{
    Planned = 1,
    InProgress = 2,
    Completed = 3,
    Approved = 4,
    Cancelled = 5
}

public enum StockCountType
{
    Full = 1,             // Tam sayım
    Cycle = 2,            // Döngüsel sayım
    Spot = 3,             // Noktasal sayım
    Annual = 4            // Yıllık sayım
}

public enum SalaryType
{
    Monthly = 1,
    Hourly = 2,
    Daily = 3,
    Annual = 4
}
/// <summary>
/// Performans Değerlendirme Derecelendirmesi Enum
/// </summary>
public enum PerformanceRating
{
    Unsatisfactory = 1,        // Yetersiz (0-59)
    NeedsImprovement = 2,      // Geliştirilmeli (60-69)
    MeetsExpectations = 3,     // Beklentileri karşılıyor (70-79)
    ExceedsExpectations = 4,   // Beklentilerin üstünde (80-89)
    Outstanding = 5            // Mükemmel (90-100)
}