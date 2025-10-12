using UniversityMS.Domain.Entities.Common;
using UniversityMS.Domain.Enums;
using UniversityMS.Domain.Exceptions;

namespace UniversityMS.Domain.Entities.EnrollmentAggregate;

public class GradeObjection : AuditableEntity
{
    public Guid GradeId { get; private set; }
    public Guid StudentId { get; private set; }
    public Guid CourseId { get; private set; }
    public string Reason { get; private set; }
    public ObjectionStatus Status { get; private set; }
    public DateTime ObjectionDate { get; private set; }
    public Guid? ReviewedBy { get; private set; }
    public DateTime? ReviewDate { get; private set; }
    public string? ReviewNotes { get; private set; }
    public double? OldScore { get; private set; }
    public double? NewScore { get; private set; }

    // Navigation Properties
    public Grade Grade { get; private set; } = null!;

    private GradeObjection() { } // EF Core

    private GradeObjection(Guid gradeId, Guid studentId, Guid courseId, string reason)
        : base()
    {
        GradeId = gradeId;
        StudentId = studentId;
        CourseId = courseId;
        Reason = reason;
        Status = ObjectionStatus.Pending;
        ObjectionDate = DateTime.UtcNow;
    }

    public static GradeObjection Create(Guid gradeId, Guid studentId, Guid courseId, string reason)
    {
        if (gradeId == Guid.Empty)
            throw new DomainException("Not ID geçersiz.");

        if (studentId == Guid.Empty)
            throw new DomainException("Öğrenci ID geçersiz.");

        if (string.IsNullOrWhiteSpace(reason))
            throw new DomainException("İtiraz nedeni belirtilmelidir.");

        return new GradeObjection(gradeId, studentId, courseId, reason);
    }

    public void Approve(Guid reviewedBy, double newScore, string? notes = null)
    {
        if (Status != ObjectionStatus.Pending)
            throw new DomainException("Sadece beklemedeki itirazlar onaylanabilir.");

        if (newScore < 0 || newScore > 100)
            throw new DomainException("Yeni not 0-100 arasında olmalıdır.");

        Status = ObjectionStatus.Approved;
        ReviewedBy = reviewedBy;
        ReviewDate = DateTime.UtcNow;
        ReviewNotes = notes;
        NewScore = newScore;
    }

    public void Reject(Guid reviewedBy, string? notes = null)
    {
        if (Status != ObjectionStatus.Pending)
            throw new DomainException("Sadece beklemedeki itirazlar reddedilebilir.");

        Status = ObjectionStatus.Rejected;
        ReviewedBy = reviewedBy;
        ReviewDate = DateTime.UtcNow;
        ReviewNotes = notes;
    }

    public void SetUnderReview()
    {
        if (Status != ObjectionStatus.Pending)
            throw new DomainException("Sadece beklemedeki itirazlar incelemeye alınabilir.");

        Status = ObjectionStatus.UnderReview;
    }
}