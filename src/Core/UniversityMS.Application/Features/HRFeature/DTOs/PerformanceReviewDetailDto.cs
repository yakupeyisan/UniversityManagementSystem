namespace UniversityMS.Application.Features.HRFeature.DTOs;

/// <summary>
/// Performans değerlendirme detay DTO'su
/// </summary>
public class PerformanceReviewDetailDto
{
    public Guid Id { get; set; }
    public Guid EmployeeId { get; set; }
    public string EmployeeName { get; set; } = null!;
    public string EmployeeNumber { get; set; } = null!;
    public Guid ReviewerId { get; set; }
    public string ReviewerName { get; set; } = null!;
    public string ReviewPeriod { get; set; } = null!;
    public DateTime ReviewDate { get; set; }
    public string Status { get; set; } = null!;

    public decimal QualityOfWorkScore { get; set; }
    public decimal ProductivityScore { get; set; }
    public decimal TeamworkScore { get; set; }
    public decimal CommunicationScore { get; set; }
    public decimal LeadershipScore { get; set; }
    public decimal OverallScore { get; set; }
    public string OverallRating { get; set; } = null!;

    public string? Strengths { get; set; }
    public string? AreasForImprovement { get; set; }
    public string? Goals { get; set; }
    public string? ReviewerComments { get; set; }
    public string? EmployeeComments { get; set; }

    public Guid? ApprovedBy { get; set; }
    public string? ApprovedByName { get; set; }
    public DateTime? ApprovedDate { get; set; }

    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}