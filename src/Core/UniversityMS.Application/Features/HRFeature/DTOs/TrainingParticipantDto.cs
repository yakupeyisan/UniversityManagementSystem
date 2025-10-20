namespace UniversityMS.Application.Features.HRFeature.DTOs;

/// <summary>
/// Eğitim katılımcısı DTO'su
/// </summary>
public class TrainingParticipantDto
{
    public Guid Id { get; set; }
    public Guid EmployeeId { get; set; }
    public string EmployeeName { get; set; } = null!;
    public string EmployeeNumber { get; set; } = null!;
    public string Status { get; set; } = null!; // Enrolled, Completed, Cancelled
    public DateTime EnrollmentDate { get; set; }
    public DateTime? CompletionDate { get; set; }
    public int? Score { get; set; }
    public bool? Passed { get; set; }
}