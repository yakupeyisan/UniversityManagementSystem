namespace UniversityMS.Application.Features.HRFeature.DTOs;

/// <summary>
/// Eğitim katılımcısı DTO'su (TrainingEnrollment mapping)
/// </summary>
public class TrainingEnrollmentDto
{
    public Guid Id { get; set; }
    public Guid EmployeeId { get; set; }
    public string EmployeeName { get; set; } = null!;
    public string EmployeeNumber { get; set; } = null!;
    public string Status { get; set; } = null!;
    public DateTime EnrollmentDate { get; set; }
    public DateTime? CompletionDate { get; set; }
    public decimal? Score { get; set; }
    public bool CertificateIssued { get; set; }
    public string? Feedback { get; set; }
}