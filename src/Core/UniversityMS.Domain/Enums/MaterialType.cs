namespace UniversityMS.Domain.Enums;

/// <summary>
/// Kütüphane Materyali Tipi
/// </summary>
public enum MaterialType
{
    Book = 1,                 // Kitap
    Magazine = 2,             // Dergi
    Journal = 3,              // Akademik Dergi
    Newspaper = 4,            // Gazete
    Thesis = 5,               // Tez
    EBook = 6,                // E-Kitap
    AudioBook = 7,            // Sesli Kitap
    DVD = 8,                  // DVD
    CD = 9,                   // CD
    Map = 10,                 // Harita
    Manuscript = 11,          // El Yazması
    ReferenceBook = 12,       // Kaynak Kitap
    ResearchPaper = 13,       // Araştırma Makalesi
    ProjectReport = 14,       // Proje Raporu
    Other = 99                // Diğer
}