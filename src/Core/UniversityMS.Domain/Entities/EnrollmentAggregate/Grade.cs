using UniversityMS.Domain.Entities.Common;
using UniversityMS.Domain.Enums;
using UniversityMS.Domain.Events;
using UniversityMS.Domain.Exceptions;
using UniversityMS.Domain.ValueObjects;

namespace UniversityMS.Domain.Entities.EnrollmentAggregate;

public class Grade : AuditableEntity
{
    public Guid CourseRegistrationId { get; private set; }
    public Guid StudentId { get; private set; }
    public Guid CourseId { get; private set; }
    public Guid? InstructorId { get; private set; }
    public GradeType GradeType { get; private set; }
    public double NumericScore { get; private set; }
    public string LetterGrade { get; private set; }
    public double GradePoint { get; private set; }
    public double Weight { get; private set; } // Ağırlık (örn: 0.30 = %30)
    public DateTime GradeDate { get; private set; }
    public string? Notes { get; private set; }

    // Navigation Properties
    public CourseRegistration CourseRegistration { get; private set; } = null!;

    private Grade() { } // EF Core

    private Grade(Guid courseRegistrationId, Guid studentId, Guid courseId,
        GradeType gradeType, double numericScore, double weight, Guid? instructorId = null)
        : base()
    {
        CourseRegistrationId = courseRegistrationId;
        StudentId = studentId;
        CourseId = courseId;
        GradeType = gradeType;
        NumericScore = numericScore;
        Weight = weight;
        InstructorId = instructorId;
        GradeDate = DateTime.UtcNow;

        var gradeScore = GradeScore.Create(numericScore);
        LetterGrade = gradeScore.LetterGrade;
        GradePoint = gradeScore.GradePoint;
    }

    public static Grade Create(Guid courseRegistrationId, Guid studentId, Guid courseId,
        GradeType gradeType, double numericScore, double weight, Guid? instructorId = null)
    {
        if (numericScore < 0 || numericScore > 100)
            throw new DomainException("Not 0-100 arasında olmalıdır.");

        if (weight < 0 || weight > 1)
            throw new DomainException("Ağırlık 0-1 arasında olmalıdır.");

        var grade = new Grade(courseRegistrationId, studentId, courseId, gradeType, numericScore, weight, instructorId);

        // Domain Event
        grade.AddDomainEvent(new GradeSubmittedEvent(studentId, courseId, numericScore, grade.LetterGrade, grade.GradePoint));

        return grade;
    }

    public void Update(double numericScore, string? notes = null)
    {
        if (numericScore < 0 || numericScore > 100)
            throw new DomainException("Not 0-100 arasında olmalıdır.");

        NumericScore = numericScore;
        Notes = notes;

        var gradeScore = GradeScore.Create(numericScore);
        LetterGrade = gradeScore.LetterGrade;
        GradePoint = gradeScore.GradePoint;
    }

    public double GetWeightedScore()
    {
        return NumericScore * Weight;
    }

    public double GetWeightedGradePoint()
    {
        return GradePoint * Weight;
    }
}