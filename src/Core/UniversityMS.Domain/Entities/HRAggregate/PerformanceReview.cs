using UniversityMS.Domain.Entities.Common;
using UniversityMS.Domain.Enums;
using UniversityMS.Domain.Exceptions;
using UniversityMS.Domain.ValueObjects;

namespace UniversityMS.Domain.Entities.HRAggregate;

/// <summary>
/// Performans Değerlendirme Entity
/// </summary>
public class PerformanceReview : AuditableEntity
{
    public Guid EmployeeId { get; private set; }
    public Guid ReviewerId { get; private set; }
    public string ReviewPeriod { get; private set; } = null!; // e.g., "2024-Q1", "2024-Annual"
    public DateTime ReviewDate { get; private set; }
    public PerformanceReviewStatus Status { get; private set; }
    public PerformanceScore OverallScore { get; private set; } = null!;
    public PerformanceRating OverallRating { get; private set; }

    // Değerlendirme kriterleri skorları
    public decimal QualityOfWorkScore { get; private set; }
    public decimal ProductivityScore { get; private set; }
    public decimal TeamworkScore { get; private set; }
    public decimal CommunicationScore { get; private set; }
    public decimal LeadershipScore { get; private set; }

    public string? Strengths { get; private set; }
    public string? AreasForImprovement { get; private set; }
    public string? Goals { get; private set; }
    public string? ReviewerComments { get; private set; }
    public string? EmployeeComments { get; private set; }

    public Guid? ApprovedBy { get; private set; }
    public DateTime? ApprovedDate { get; private set; }

    // Navigation Properties
    public Employee Employee { get; private set; } = null!;
    public Employee Reviewer { get; private set; } = null!;
    public Employee? Approver { get; private set; }

    // Parameterless constructor for EF Core
    private PerformanceReview() { }

    private PerformanceReview(
        Guid employeeId,
        Guid reviewerId,
        string reviewPeriod,
        DateTime reviewDate)
    {
        EmployeeId = employeeId;
        ReviewerId = reviewerId;
        ReviewPeriod = reviewPeriod;
        ReviewDate = reviewDate;
        Status = PerformanceReviewStatus.Scheduled;
    }

    public static PerformanceReview Create(
        Guid employeeId,
        Guid reviewerId,
        string reviewPeriod,
        DateTime reviewDate)
    {
        if (string.IsNullOrWhiteSpace(reviewPeriod))
            throw new DomainException("Değerlendirme dönemi belirtilmelidir.");

        if (reviewDate > DateTime.UtcNow)
            throw new DomainException("Değerlendirme tarihi gelecekte olamaz.");

        return new PerformanceReview(employeeId, reviewerId, reviewPeriod, reviewDate);
    }

    public void StartReview()
    {
        if (Status != PerformanceReviewStatus.Scheduled)
            throw new DomainException("Sadece planlanmış değerlendirmeler başlatılabilir.");

        Status = PerformanceReviewStatus.InProgress;
    }

    public void CompleteReview(
        decimal qualityOfWorkScore,
        decimal productivityScore,
        decimal teamworkScore,
        decimal communicationScore,
        decimal leadershipScore,
        string? strengths = null,
        string? areasForImprovement = null,
        string? goals = null,
        string? reviewerComments = null)
    {
        if (Status != PerformanceReviewStatus.InProgress)
            throw new DomainException("Sadece devam eden değerlendirmeler tamamlanabilir.");

        ValidateScore(qualityOfWorkScore, nameof(qualityOfWorkScore));
        ValidateScore(productivityScore, nameof(productivityScore));
        ValidateScore(teamworkScore, nameof(teamworkScore));
        ValidateScore(communicationScore, nameof(communicationScore));
        ValidateScore(leadershipScore, nameof(leadershipScore));

        QualityOfWorkScore = qualityOfWorkScore;
        ProductivityScore = productivityScore;
        TeamworkScore = teamworkScore;
        CommunicationScore = communicationScore;
        LeadershipScore = leadershipScore;

        // Genel skoru hesapla (ağırlıklı ortalama)
        var overallScoreValue = (qualityOfWorkScore * 0.3m) +
                                (productivityScore * 0.25m) +
                                (teamworkScore * 0.2m) +
                                (communicationScore * 0.15m) +
                                (leadershipScore * 0.1m);

        OverallScore = PerformanceScore.Create(overallScoreValue);
        OverallRating = OverallScore.GetRating();

        Strengths = strengths;
        AreasForImprovement = areasForImprovement;
        Goals = goals;
        ReviewerComments = reviewerComments;

        Status = PerformanceReviewStatus.Completed;
    }

    public void AddEmployeeComments(string comments)
    {
        if (Status != PerformanceReviewStatus.Completed)
            throw new DomainException("Sadece tamamlanmış değerlendirmelere çalışan yorumu eklenebilir.");

        EmployeeComments = comments;
        Status = PerformanceReviewStatus.PendingApproval;
    }

    public void Approve(Guid approverId)
    {
        if (Status != PerformanceReviewStatus.PendingApproval && Status != PerformanceReviewStatus.Completed)
            throw new DomainException("Değerlendirme onaylanamaz.");

        ApprovedBy = approverId;
        ApprovedDate = DateTime.UtcNow;
        Status = PerformanceReviewStatus.Approved;
    }

    public void Cancel()
    {
        if (Status == PerformanceReviewStatus.Approved)
            throw new DomainException("Onaylı değerlendirme iptal edilemez.");

        Status = PerformanceReviewStatus.Cancelled;
    }

    private void ValidateScore(decimal score, string scoreName)
    {
        if (score < 0 || score > 100)
            throw new DomainException($"{scoreName} 0-100 arasında olmalıdır.");
    }

    public bool IsExcellent() => OverallScore >= 90;
    public bool IsGood() => OverallScore >= 70;
    public bool NeedsImprovement() => OverallScore < 60;
}