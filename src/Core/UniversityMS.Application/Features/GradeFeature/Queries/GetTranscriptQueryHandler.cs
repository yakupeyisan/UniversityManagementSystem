using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using UniversityMS.Application.Common.Models;
using UniversityMS.Application.Features.GradeFeature.DTOs;
using UniversityMS.Domain.Entities.EnrollmentAggregate;
using UniversityMS.Domain.Entities.PersonAggregate;
using UniversityMS.Domain.Interfaces;

namespace UniversityMS.Application.Features.GradeFeature.Queries;

public class GetTranscriptQueryHandler : IRequestHandler<GetTranscriptQuery, Result<TranscriptDto>>
{
    private readonly IRepository<Grade> _gradeRepository;
    private readonly IRepository<Enrollment> _enrollmentRepository;
    private readonly IMapper _mapper;
    private readonly ILogger<GetTranscriptQueryHandler> _logger;

    public GetTranscriptQueryHandler(
        IRepository<Grade> gradeRepository,
        IRepository<Enrollment> enrollmentRepository,
        IMapper mapper,
        ILogger<GetTranscriptQueryHandler> logger)
    {
        _gradeRepository = gradeRepository;
        _enrollmentRepository = enrollmentRepository;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<Result<TranscriptDto>> Handle(
        GetTranscriptQuery request,
        CancellationToken cancellationToken)
    {
        try
        {
            var enrollments = await _enrollmentRepository.FindAsync(
                e => e.StudentId == request.StudentId,
                cancellationToken);

            if (!enrollments.Any())
                return Result<TranscriptDto>.Failure("Öğrenci kaydı bulunamadı.");

            var grades = new List<Grade>();
            foreach (var enrollment in enrollments)
            {
                var enrollmentGrades = await _gradeRepository.FindAsync(
                    g => g.CourseRegistration.EnrollmentId == enrollment.Id,
                    cancellationToken);
                grades.AddRange(enrollmentGrades);
            }

            var gpa = CalculateGPA(grades);
            var transcript = new TranscriptDto
            {
                StudentId = request.StudentId,
                GPA = gpa,
                TotalCredits = grades.Sum(g => g.CourseRegistration.Course.Credits),
                CompletedCredits = grades.Where(g => g.NumericGrade >= 50)
                    .Sum(g => g.CourseRegistration.Course.Credits),
                TotalCourses = grades.Count,
                PassedCourses = grades.Count(g => g.NumericGrade >= 50),
                FailedCourses = grades.Count(g => g.NumericGrade < 50),
                AverageGrade = grades.Where(g => g.NumericGrade.HasValue)
                    .Average(g => g.NumericGrade) ?? 0,
                Courses = _mapper.Map<List<TranscriptCourseDto>>(grades.OrderByDescending(g => g.CreatedAt)),
                GeneratedDate = DateTime.UtcNow
            };

            return Result<TranscriptDto>.Success(transcript);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error generating transcript");
            return Result<TranscriptDto>.Failure("Transkript oluşturulurken hata oluştu.", ex.Message);
        }
    }

    private decimal CalculateGPA(List<Grade> grades)
    {
        if (!grades.Any()) return 0;
        var passedGrades = grades.Where(g => g.NumericGrade >= 50).ToList();
        if (!passedGrades.Any()) return 0;
        return passedGrades.Average(g => g.NumericGrade.GetValueOrDefault() / 20m);
    }
}