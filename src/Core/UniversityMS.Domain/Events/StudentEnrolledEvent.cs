using UniversityMS.Domain.Interfaces;
using UniversityMS.Domain.ValueObjects;

namespace UniversityMS.Domain.Events;

public class StudentEnrolledEvent : BaseDomainEvent
{
    public Guid StudentId { get; }
    public string StudentNumber { get; }
    public Guid DepartmentId { get; }

    public StudentEnrolledEvent(Guid studentId, string studentNumber, Guid departmentId)
    {
        StudentId = studentId;
        StudentNumber = studentNumber;
        DepartmentId = departmentId;
    }
}
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
public class PaymentReceivedEvent : BaseDomainEvent
{
    public Guid StudentId { get; }
    public Money Amount { get; }
    public string PaymentMethod { get; }
    public string TransactionId { get; }

    public PaymentReceivedEvent(Guid studentId, Money amount,
        string paymentMethod, string transactionId)
    {
        StudentId = studentId;
        Amount = amount;
        PaymentMethod = paymentMethod;
        TransactionId = transactionId;
    }
}