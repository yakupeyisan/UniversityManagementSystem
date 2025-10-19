namespace UniversityMS.Application.Common.Interfaces;

public interface IGradeCalculationService
{
    decimal CalculateGPA(List<(int credits, decimal grade)> grades);
    string GetLetterGrade(decimal numericGrade);
    decimal CalculateWeightedGrade(List<(decimal weight, decimal grade)> weightedGrades);
    bool IsPassingGrade(decimal numericGrade);
}