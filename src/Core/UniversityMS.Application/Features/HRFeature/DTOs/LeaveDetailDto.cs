namespace UniversityMS.Application.Features.HRFeature.DTOs;

/// <summary>
/// İzin Detayı DTO
/// İzin talebinin tüm bilgilerini içerir
/// </summary>
public class LeaveDetailDto
{
    // ========== TEMEL BİLGİLER ==========
    public Guid Id { get; set; }
    public Guid EmployeeId { get; set; }
    public string EmployeeName { get; set; } = null!;
    public string EmployeeNumber { get; set; } = null!;
    public string Department { get; set; } = null!;

    // ========== İZİN BİLGİLERİ ==========
    public string LeaveType { get; set; } = null!;
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public int DurationDays { get; set; }
    public string Reason { get; set; } = null!;

    // ========== DURUM BİLGİLERİ ==========
    public string Status { get; set; } = null!;
    public DateTime RequestDate { get; set; }
    public DateTime? ApprovedDate { get; set; }
    public string? ApprovedBy { get; set; }
    public DateTime? RejectedDate { get; set; }
    public string? RejectionReason { get; set; }
    public string? RejectedBy { get; set; }

    // ========== EK BİLGİLER ==========
    public string? DocumentPath { get; set; }
    public string? Notes { get; set; }
}