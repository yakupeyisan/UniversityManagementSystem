using UniversityMS.Domain.Entities.Common;
using UniversityMS.Domain.Enums;
using UniversityMS.Domain.Exceptions;

namespace UniversityMS.Domain.Entities.HRAggregate;

/// <summary>
/// Eğitim Kayıt Entity (Training ile Many-to-Many ilişki için)
/// </summary>
public class TrainingEnrollment : BaseEntity
{
    public Guid TrainingId { get; private set; }
    public Guid EmployeeId { get; private set; }
    public DateTime EnrollmentDate { get; private set; }
    public TrainingEnrollmentStatus Status { get; private set; }
    public DateTime? CompletionDate { get; private set; }
    public decimal? Score { get; private set; }
    public bool CertificateIssued { get; private set; }
    public string? Feedback { get; private set; }
    public bool IsCompleted { get; private set; }
    public string? Certificate { get; private set; }

    // Navigation Properties
    public Training Training { get; private set; } = null!;
    public Employee Employee { get; private set; } = null!;

    private TrainingEnrollment() { }

    private TrainingEnrollment(Guid trainingId, Guid employeeId)
    {
        TrainingId = trainingId;
        EmployeeId = employeeId;
        EnrollmentDate = DateTime.UtcNow;
        Status = TrainingEnrollmentStatus.Enrolled;
    }

    public static TrainingEnrollment Create(Guid trainingId, Guid employeeId)
    {
        return new TrainingEnrollment(trainingId, employeeId);
    }

    public void MarkAsCompleted(decimal? score = null, string? feedback = null)
    {
        if (Status == TrainingEnrollmentStatus.Completed)
            throw new DomainException("Eğitim zaten tamamlanmış.");

        Status = TrainingEnrollmentStatus.Completed;
        CompletionDate = DateTime.UtcNow;
        Score = score;
        Feedback = feedback;
    }

    public void IssueCertificate()
    {
        if (Status != TrainingEnrollmentStatus.Completed)
            throw new DomainException("Sadece tamamlanmış eğitimler için sertifika verilebilir.");

        CertificateIssued = true;
    }
    public void Complete(decimal? score = null, string? certificate = null)
    {
        if (IsCompleted)
            throw new DomainException("Eğitim zaten tamamlanmış.");

        if (score.HasValue && (score < 0 || score > 100))
            throw new DomainException("Puan 0-100 arasında olmalıdır.");

        IsCompleted = true;
        CompletionDate = DateTime.UtcNow;
        Score = score;
        Certificate = certificate;
    }

    public void Cancel()
    {
        if (Status == TrainingEnrollmentStatus.Completed)
            throw new DomainException("Tamamlanmış eğitim kaydı iptal edilemez.");

        Status = TrainingEnrollmentStatus.Cancelled;
    }
}