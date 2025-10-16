using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using UniversityMS.Application.Common.Models;
using UniversityMS.Application.Features.Courses.DTOs;
using UniversityMS.Domain.Entities.AcademicAggregate;
using UniversityMS.Domain.Interfaces;

namespace UniversityMS.Application.Features.Courses.Queries;

public class GetCourseByIdQueryHandler : IRequestHandler<GetCourseByIdQuery, Result<CourseDto>>
{
    private readonly IRepository<Course> _courseRepository;
    private readonly IMapper _mapper;
    private readonly ILogger<GetCourseByIdQueryHandler> _logger;

    public GetCourseByIdQueryHandler(
        IRepository<Course> courseRepository,
        IMapper mapper,
        ILogger<GetCourseByIdQueryHandler> logger)
    {
        _courseRepository = courseRepository;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<Result<CourseDto>> Handle(GetCourseByIdQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var course = await _courseRepository.GetByIdAsync(request.Id, cancellationToken);

            if (course == null)
            {
                _logger.LogWarning("Course not found. CourseId: {CourseId}", request.Id);
                return Result<CourseDto>.Failure("Ders bulunamadı.");
            }

            var courseDto = _mapper.Map<CourseDto>(course);
            return Result<CourseDto>.Success(courseDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving course. CourseId: {CourseId}", request.Id);
            return Result<CourseDto>.Failure("Ders bilgileri alınırken bir hata oluştu.");
        }
    }
}