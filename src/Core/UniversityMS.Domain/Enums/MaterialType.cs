namespace UniversityMS.Domain.Enums;

/// <summary>
/// Malzeme Türü - Harmonized (Mevcut + Yeni kombinasyon)
/// </summary>
public enum MaterialType
{
    Book = 0,                // Kitap
    Magazine = 1,            // Dergi
    Journal = 2,             // Akademik dergi
    Newspaper = 3,           // Gazete
    Thesis = 4,              // Tez
    EBook = 5,               // E-Kitap
    AudioBook = 6,           // Sesli kitap
    DVD = 7,                 // DVD
    CD = 8,                  // CD
    BlueRay = 9,             // Blu-Ray
    Map = 10,                // Harita
    Manuscript = 11,         // El yazması
    ReferenceBook = 12,      // Kaynak kitap
    ResearchPaper = 13,      // Araştırma makalesi
    ProjectReport = 14,      // Proje raporu
    OnlineResource = 15,     // Çevrimiçi kaynak
    Other = 99               // Diğer
}