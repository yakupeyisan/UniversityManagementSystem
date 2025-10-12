using UniversityMS.Domain.Exceptions;

namespace UniversityMS.Domain.ValueObjects;

public sealed class GradeScore : ValueObject
{
    public double NumericScore { get; }
    public string LetterGrade { get; }
    public double GradePoint { get; }

    private GradeScore(double numericScore, string letterGrade, double gradePoint)
    {
        NumericScore = numericScore;
        LetterGrade = letterGrade;
        GradePoint = gradePoint;
    }

    public static GradeScore Create(double numericScore)
    {
        if (numericScore < 0 || numericScore > 100)
            throw new DomainException("Not 0-100 arasında olmalıdır.");

        var (letterGrade, gradePoint) = CalculateLetterGrade(numericScore);
        return new GradeScore(numericScore, letterGrade, gradePoint);
    }

    private static (string Letter, double Point) CalculateLetterGrade(double score)
    {
        return score switch
        {
            >= 90 => ("AA", 4.0),
            >= 85 => ("BA", 3.5),
            >= 80 => ("BB", 3.0),
            >= 75 => ("CB", 2.5),
            >= 70 => ("CC", 2.0),
            >= 65 => ("DC", 1.5),
            >= 60 => ("DD", 1.0),
            >= 50 => ("FD", 0.5),
            _ => ("FF", 0.0)
        };
    }

    public bool IsPassing() => GradePoint >= 2.0;

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return NumericScore;
        yield return LetterGrade;
        yield return GradePoint;
    }

    public override string ToString() => $"{NumericScore:F2} ({LetterGrade})";
}