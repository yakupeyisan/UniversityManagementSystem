using UniversityMS.Domain.Interfaces;

namespace UniversityMS.Domain.Events;

public class GradeSubmittedEvent : BaseDomainEvent
{
    public Guid StudentId { get; }
    public Guid CourseId { get; }
    public double NumericScore { get; }
    public string LetterGrade { get; }
    public double GradePoint { get; }

    public GradeSubmittedEvent(Guid studentId, Guid courseId,
        double numericScore, string letterGrade, double gradePoint)
    {
        StudentId = studentId;
        CourseId = courseId;
        NumericScore = numericScore;
        LetterGrade = letterGrade;
        GradePoint = gradePoint;
    }
}