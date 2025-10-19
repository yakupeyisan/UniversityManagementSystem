using MediatR;
using Microsoft.Extensions.Logging;
using UniversityMS.Application.Common.Models;
using UniversityMS.Application.Features.CourseFeature.DTOs;
using UniversityMS.Domain.Entities.AcademicAggregate;
using UniversityMS.Domain.Interfaces;

namespace UniversityMS.Application.Features.CourseFeature.Queries;

public class GetCoursePrerequisitesQueryHandler : IRequestHandler<GetCoursePrerequisitesQuery, Result<List<PrerequisiteCourseDto>>>
{
    private readonly IRepository<Course> _courseRepository;
    private readonly ILogger<GetCoursePrerequisitesQueryHandler> _logger;

    public GetCoursePrerequisitesQueryHandler(
        IRepository<Course> courseRepository,
        ILogger<GetCoursePrerequisitesQueryHandler> logger)
    {
        _courseRepository = courseRepository;
        _logger = logger;
    }

    public async Task<Result<List<PrerequisiteCourseDto>>> Handle(
        GetCoursePrerequisitesQuery request,
        CancellationToken cancellationToken)
    {
        try
        {
            var course = await _courseRepository.GetByIdAsync(request.CourseId, cancellationToken);

            if (course == null)
            {
                _logger.LogWarning("Course not found. CourseId: {CourseId}", request.CourseId);
                return Result<List<PrerequisiteCourseDto>>.Failure("Ders bulunamadı.");
            }

            var prerequisites = course.Prerequisites
                .Select(p => new PrerequisiteCourseDto
                {
                    CourseId = p.PrerequisiteCourseId,
                    CourseName = p.PrerequisiteCourse.Name,
                    CourseCode = p.PrerequisiteCourse.Code
                })
                .ToList();

            return Result<List<PrerequisiteCourseDto>>.Success(prerequisites);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving course prerequisites. CourseId: {CourseId}", request.CourseId);
            return Result<List<PrerequisiteCourseDto>>.Failure("Ön koşul dersleri alınırken bir hata oluştu.");
        }
    }
}