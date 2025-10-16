using System.Linq.Expressions;
using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using UniversityMS.Application.Common.Extensions;
using UniversityMS.Application.Common.Models;
using UniversityMS.Application.Features.Courses.DTOs;
using UniversityMS.Domain.Entities.AcademicAggregate;
using UniversityMS.Domain.Interfaces;

namespace UniversityMS.Application.Features.Courses.Queries;

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

            return Result<PaginatedList<CourseDto>>.Success(paginatedList);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving course list");
            return Result<PaginatedList<CourseDto>>.Failure("Ders listesi alınırken bir hata oluştu.");
        }
    }
}