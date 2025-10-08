using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using System.Linq.Expressions;
using UniversityMS.Application.Common.Extensions;
using UniversityMS.Application.Common.Models;
using UniversityMS.Application.Features.Courses.DTOs;
using UniversityMS.Domain.Entities.AcademicAggregate;
using UniversityMS.Domain.Interfaces;

namespace UniversityMS.Application.Features.Courses.Queries;


public record GetCourseListQuery(
    int PageNumber = 1,
    int PageSize = 10,
    Guid? DepartmentId = null,
    string? SearchTerm = null,
    bool? IsActive = null
) : IRequest<Result<PaginatedList<CourseDto>>>;

public class GetCourseListQueryHandler : IRequestHandler<GetCourseListQuery, Result<PaginatedList<CourseDto>>>
{
    private readonly IRepository<Course> _courseRepository;
    private readonly IMapper _mapper;
    private readonly ILogger<GetCourseListQueryHandler> _logger;

    public GetCourseListQueryHandler(
        IRepository<Course> courseRepository,
        IMapper mapper,
        ILogger<GetCourseListQueryHandler> logger)
    {
        _courseRepository = courseRepository;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<Result<PaginatedList<CourseDto>>> Handle(
        GetCourseListQuery request,
        CancellationToken cancellationToken)
    {
        try
        {
            Expression<Func<Course, bool>> predicate = c => !c.IsDeleted;

            if (request.DepartmentId.HasValue)
            {
                var deptId = request.DepartmentId.Value;
                predicate = predicate.And(c => c.DepartmentId == deptId);
            }

            if (request.IsActive.HasValue)
            {
                var isActive = request.IsActive.Value;
                predicate = predicate.And(c => c.IsActive == isActive);
            }

            if (!string.IsNullOrWhiteSpace(request.SearchTerm))
            {
                var searchTerm = request.SearchTerm.ToLower();
                predicate = predicate.And(c =>
                    c.Name.ToLower().Contains(searchTerm) ||
                    c.Code.ToLower().Contains(searchTerm));
            }

            var (courses, totalCount) = await _courseRepository.GetPagedAsync(
                request.PageNumber,
                request.PageSize,
                predicate,
                cancellationToken);

            var courseDtos = _mapper.Map<List<CourseDto>>(courses);
            var paginatedList = new PaginatedList<CourseDto>(
                courseDtos,
                totalCount,
                request.PageNumber,
                request.PageSize);

            return Result.Success(paginatedList);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving course list");
            return Result.Failure<PaginatedList<CourseDto>>("Ders listesi alınırken bir hata oluştu.");
        }
    }
}
public record GetCourseByIdQuery(Guid Id) : IRequest<Result<CourseDto>>;

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
                return Result.Failure<CourseDto>("Ders bulunamadı.");
            }

            var courseDto = _mapper.Map<CourseDto>(course);
            return Result.Success(courseDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving course. CourseId: {CourseId}", request.Id);
            return Result.Failure<CourseDto>("Ders bilgileri alınırken bir hata oluştu.");
        }
    }
}

// ===== GetCoursePrerequisitesQuery =====
public record GetCoursePrerequisitesQuery(Guid CourseId) : IRequest<Result<List<PrerequisiteCourseDto>>>;

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
                return Result.Failure<List<PrerequisiteCourseDto>>("Ders bulunamadı.");
            }

            var prerequisites = course.Prerequisites
                .Select(p => new PrerequisiteCourseDto
                {
                    CourseId = p.PrerequisiteCourseId,
                    CourseName = p.PrerequisiteCourse.Name,
                    CourseCode = p.PrerequisiteCourse.Code
                })
                .ToList();

            return Result.Success(prerequisites);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving course prerequisites. CourseId: {CourseId}", request.CourseId);
            return Result.Failure<List<PrerequisiteCourseDto>>("Ön koşul dersleri alınırken bir hata oluştu.");
        }
    }
}
