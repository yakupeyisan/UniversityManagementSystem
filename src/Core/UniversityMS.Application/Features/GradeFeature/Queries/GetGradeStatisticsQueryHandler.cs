using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using UniversityMS.Application.Common.Models;
using UniversityMS.Application.Features.GradeFeature.DTOs;
using UniversityMS.Domain.Entities.EnrollmentAggregate;
using UniversityMS.Domain.Interfaces;

namespace UniversityMS.Application.Features.GradeFeature.Queries;

public class GetGradeStatisticsQueryHandler : IRequestHandler<GetGradeStatisticsQuery, Result<GradeStatisticsDto>>
{
    private readonly IRepository<Grade> _gradeRepository;
    private readonly IMapper _mapper;
    private readonly ILogger<GetGradeStatisticsQueryHandler> _logger;

    public GetGradeStatisticsQueryHandler(
        IRepository<Grade> gradeRepository,
        IMapper mapper,
        ILogger<GetGradeStatisticsQueryHandler> logger)
    {
        _gradeRepository = gradeRepository;
        _mapper = mapper;
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
                return Result<GradeStatisticsDto>.Failure("Bu ders için not bulunamadı.");

            // NumericScore'ları al (NumericGrade değil!)
            var gradeValues = grades.Select(g => g.NumericScore).ToList();

            var stats = new GradeStatisticsDto
            {
                CourseId = request.CourseId,
                TotalStudents = grades.Count,
                AverageGrade = gradeValues.Any() ? gradeValues.Average() : 0,
                HighestGrade = gradeValues.Any() ? gradeValues.Max() : 0,
                LowestGrade = gradeValues.Any() ? gradeValues.Min() : 0,
                MedianGrade = CalculateMedian(gradeValues),
                PassCount = grades.Count(g => g.GradePoint >= 2.0),
                FailCount = grades.Count(g => g.GradePoint < 2.0),
                Grades = _mapper.Map<List<GradeDetailDto>>(grades)
            };

            return Result<GradeStatisticsDto>.Success(stats);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error calculating grade statistics");
            return Result<GradeStatisticsDto>.Failure("Not istatistikleri hesaplanırken hata oluştu.");
        }
    }

    private decimal CalculateMedian(List<double> values)
    {
        if (!values.Any()) return 0;
        var sorted = values.OrderBy(x => x).ToList();
        int count = sorted.Count;
        if (count % 2 == 0)
            return (decimal)((sorted[count / 2 - 1] + sorted[count / 2]) / 2);
        return (decimal)sorted[count / 2];
    }
}
