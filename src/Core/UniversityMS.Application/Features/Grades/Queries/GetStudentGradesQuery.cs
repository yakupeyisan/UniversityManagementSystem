using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using System.Linq.Expressions;
using UniversityMS.Application.Common.Extensions;
using UniversityMS.Application.Common.Models;
using UniversityMS.Application.Features.Grades.DTOs;
using UniversityMS.Domain.Entities.EnrollmentAggregate;
using UniversityMS.Domain.Entities.PersonAggregate;
using UniversityMS.Domain.Interfaces;

namespace UniversityMS.Application.Features.Grades.Queries;

public record GetStudentGradesQuery(
    Guid StudentId,
    Guid? CourseId = null
) : IRequest<Result<List<GradeDto>>>;

public class GetStudentGradesQueryHandler : IRequestHandler<GetStudentGradesQuery, Result<List<GradeDto>>>
{
    private readonly IRepository<Grade> _gradeRepository;
    private readonly IMapper _mapper;
    private readonly ILogger<GetStudentGradesQueryHandler> _logger;

    public GetStudentGradesQueryHandler(
        IRepository<Grade> gradeRepository,
        IMapper mapper,
        ILogger<GetStudentGradesQueryHandler> logger)
    {
        _gradeRepository = gradeRepository;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<Result<List<GradeDto>>> Handle(GetStudentGradesQuery request, CancellationToken cancellationToken)
    {
        try
        {
            Expression<Func<Grade, bool>> predicate = g => g.StudentId == request.StudentId;

            if (request.CourseId.HasValue)
            {
                var courseId = request.CourseId.Value;
                predicate = predicate.And(g => g.CourseId == courseId);
            }

            var grades = await _gradeRepository.FindAsync(predicate, cancellationToken);
            var gradeDtos = _mapper.Map<List<GradeDto>>(grades);

            return Result.Success(gradeDtos);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving student grades. StudentId: {StudentId}", request.StudentId);
            return Result.Failure<List<GradeDto>>("Öğrenci notları alınırken bir hata oluştu.");
        }
    }
}

// ===== GetCourseGradesQuery =====
public record GetCourseGradesQuery(
    Guid CourseId,
    int PageNumber = 1,
    int PageSize = 50
) : IRequest<Result<PaginatedList<GradeDto>>>;

public class GetCourseGradesQueryHandler : IRequestHandler<GetCourseGradesQuery, Result<PaginatedList<GradeDto>>>
{
    private readonly IRepository<Grade> _gradeRepository;
    private readonly IMapper _mapper;
    private readonly ILogger<GetCourseGradesQueryHandler> _logger;

    public GetCourseGradesQueryHandler(
        IRepository<Grade> gradeRepository,
        IMapper mapper,
        ILogger<GetCourseGradesQueryHandler> logger)
    {
        _gradeRepository = gradeRepository;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<Result<PaginatedList<GradeDto>>> Handle(
        GetCourseGradesQuery request,
        CancellationToken cancellationToken)
    {
        try
        {
            Expression<Func<Grade, bool>> predicate = g => g.CourseId == request.CourseId;

            var (grades, totalCount) = await _gradeRepository.GetPagedAsync(
                request.PageNumber,
                request.PageSize,
                predicate,
                cancellationToken);

            var gradeDtos = _mapper.Map<List<GradeDto>>(grades);
            var paginatedList = new PaginatedList<GradeDto>(
                gradeDtos,
                totalCount,
                request.PageNumber,
                request.PageSize);

            return Result.Success(paginatedList);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving course grades. CourseId: {CourseId}", request.CourseId);
            return Result.Failure<PaginatedList<GradeDto>>("Ders notları alınırken bir hata oluştu.");
        }
    }
}

// ===== GetGradeStatisticsQuery =====
public record GetGradeStatisticsQuery(Guid CourseId) : IRequest<Result<GradeStatisticsDto>>;

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
                return Result.Failure<GradeStatisticsDto>("Bu ders için henüz not girilmemiş.");

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

            return Result.Success(statistics);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error calculating grade statistics. CourseId: {CourseId}", request.CourseId);
            return Result.Failure<GradeStatisticsDto>("Not istatistikleri hesaplanırken bir hata oluştu.");
        }
    }
}

public record GetTranscriptQuery(Guid StudentId) : IRequest<Result<TranscriptDto>>;

public class GetTranscriptQueryHandler : IRequestHandler<GetTranscriptQuery, Result<TranscriptDto>>
{
    private readonly IRepository<Grade> _gradeRepository;
    private readonly IRepository<Student> _studentRepository;
    private readonly IMapper _mapper;
    private readonly ILogger<GetTranscriptQueryHandler> _logger;

    public GetTranscriptQueryHandler(
        IRepository<Grade> gradeRepository,
        IRepository<Student> studentRepository,
        IMapper mapper,
        ILogger<GetTranscriptQueryHandler> logger)
    {
        _gradeRepository = gradeRepository;
        _studentRepository = studentRepository;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<Result<TranscriptDto>> Handle(GetTranscriptQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var student = await _studentRepository.GetByIdAsync(request.StudentId, cancellationToken);
            if (student == null)
                return Result.Failure<TranscriptDto>("Öğrenci bulunamadı.");

            var grades = await _gradeRepository.FindAsync(
                g => g.StudentId == request.StudentId,
                cancellationToken);

            var gradeDtos = _mapper.Map<List<GradeDto>>(grades);

            var transcript = new TranscriptDto
            {
                StudentId = student.Id,
                StudentNumber = student.StudentNumber,
                FullName = $"{student.FirstName} {student.LastName}",
                CGPA = student.CGPA,
                TotalCredits = student.TotalCredits,
                CompletedCredits = student.CompletedCredits,
                Grades = gradeDtos
            };

            return Result.Success(transcript);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error generating transcript");
            return Result.Failure<TranscriptDto>("Transkript oluşturulamadı.");
        }
    }
}
