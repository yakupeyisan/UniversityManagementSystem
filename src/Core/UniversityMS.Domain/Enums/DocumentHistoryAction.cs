namespace UniversityMS.Domain.Enums;

/// <summary>
/// Belge Geçmişi İşlemi
/// </summary>
public enum DocumentHistoryAction
{
    Created = 1,
    Published = 2,
    Signed = 3,
    Delivered = 4,
    Archived = 5,
    Modified = 6
}