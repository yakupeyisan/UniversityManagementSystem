using MediatR;
using Microsoft.Extensions.Logging;
using UniversityMS.Application.Common.Models;
using UniversityMS.Application.Features.Grades.DTOs;
using UniversityMS.Domain.Entities.EnrollmentAggregate;
using UniversityMS.Domain.Interfaces;

namespace UniversityMS.Application.Features.Grades.Queries;

public class GetGradeStatisticsQueryHandler : IRequestHandler<GetGradeStatisticsQuery, Result<GradeStatisticsDto>>
{
    private readonly IRepository<Grade> _gradeRepository;
    private readonly ILogger<GetGradeStatisticsQueryHandler> _logger;

    public GetGradeStatisticsQueryHandler(
        IRepository<Grade> gradeRepository,
        ILogger<GetGradeStatisticsQueryHandler> logger)
    {
        _gradeRepository = gradeRepository;
        _logger = logger;
    }

    public async Task<Result<GradeStatisticsDto>> Handle(
        GetGradeStatisticsQuery request,
        CancellationToken cancellationToken)
    {
        try
        {
            var grades = await _gradeRepository.FindAsync(
                g => g.CourseId == request.CourseId,
                cancellationToken);

            if (!grades.Any())
                return Result<GradeStatisticsDto>.Failure("Bu ders için henüz not girilmemiş.");

            var statistics = new GradeStatisticsDto
            {
                CourseId = request.CourseId,
                TotalStudents = grades.Count,
                AverageScore = grades.Average(g => g.NumericScore),
                AverageGradePoint = grades.Average(g => g.GradePoint),
                HighestScore = grades.Max(g => g.NumericScore),
                LowestScore = grades.Min(g => g.NumericScore),
                PassRate = (double)grades.Count(g => g.GradePoint >= 2.0) / grades.Count * 100,

                GradeDistribution = grades
                    .GroupBy(g => g.LetterGrade)
                    .Select(g => new GradeDistributionDto
                    {
                        LetterGrade = g.Key,
                        Count = g.Count(),
                        Percentage = (double)g.Count() / grades.Count * 100
                    })
                    .OrderByDescending(g => g.Percentage)
                    .ToList()
            };

            return Result<GradeStatisticsDto>.Success(statistics);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error calculating grade statistics. CourseId: {CourseId}", request.CourseId);
            return Result<GradeStatisticsDto>.Failure("Not istatistikleri hesaplanırken bir hata oluştu.");
        }
    }
}