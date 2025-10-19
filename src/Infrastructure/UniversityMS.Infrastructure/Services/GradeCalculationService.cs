using UniversityMS.Application.Common.Interfaces;

namespace UniversityMS.Infrastructure.Services;

public class GradeCalculationService : IGradeCalculationService
{
    private const decimal PassingGrade = 50m;
    private const decimal FullScale = 100m;

    public decimal CalculateGPA(List<(int credits, decimal grade)> grades)
    {
        if (!grades.Any())
            return 0;

        var totalCredits = grades.Sum(g => g.credits);
        if (totalCredits == 0)
            return 0;

        var weightedSum = grades.Sum(g => g.credits * (g.grade / 20));
        return weightedSum / totalCredits;
    }

    public string GetLetterGrade(decimal numericGrade)
    {
        return numericGrade switch
        {
            >= 90 => "A",
            >= 80 => "B",
            >= 70 => "C",
            >= 60 => "D",
            >= 50 => "E",
            _ => "F"
        };
    }

    public decimal CalculateWeightedGrade(List<(decimal weight, decimal grade)> weightedGrades)
    {
        if (!weightedGrades.Any())
            return 0;

        var totalWeight = weightedGrades.Sum(wg => wg.weight);
        if (totalWeight == 0)
            return 0;

        return weightedGrades.Sum(wg => wg.weight * wg.grade) / totalWeight;
    }

    public bool IsPassingGrade(decimal numericGrade)
    {
        return numericGrade >= PassingGrade;
    }
}