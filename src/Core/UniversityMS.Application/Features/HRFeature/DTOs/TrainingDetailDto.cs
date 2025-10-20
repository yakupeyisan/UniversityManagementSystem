namespace UniversityMS.Application.Features.HRFeature.DTOs;


/// <summary>
/// Eğitim detay DTO'su
/// </summary>
public class TrainingDetailDto
{
    public Guid Id { get; set; }
    public string Title { get; set; } = null!;
    public string Description { get; set; } = null!;
    public string TrainingType { get; set; } = null!;
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public int Duration { get; set; }
    public string Location { get; set; } = null!;
    public decimal? Cost { get; set; }
    public string Status { get; set; } = null!;
    public int ParticipantCount { get; set; }
    public int MaxParticipants { get; set; }
    public string? Instructor { get; set; }
    public bool IsCertified { get; set; }
    public string? Materials { get; set; }

    public List<TrainingEnrollmentDto> Enrollments { get; set; } = new();

    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
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