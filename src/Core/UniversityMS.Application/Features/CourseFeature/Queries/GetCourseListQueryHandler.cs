using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using UniversityMS.Application.Common.Models;
using UniversityMS.Application.Features.CourseFeature.DTOs;
using UniversityMS.Domain.Entities.AcademicAggregate;
using UniversityMS.Domain.Filters;
using UniversityMS.Domain.Interfaces;
using UniversityMS.Domain.Specifications;

namespace UniversityMS.Application.Features.CourseFeature.Queries;
public class GetCourseListQueryHandler : IRequestHandler<GetCourseListQuery, Result<PaginatedList<CourseDto>>>
{
    private readonly IRepository<Course> _courseRepository;
    private readonly IFilterParser<Course> _filterParser;
    private readonly IMapper _mapper;
    private readonly ILogger<GetCourseListQueryHandler> _logger;

    public GetCourseListQueryHandler(
        IRepository<Course> courseRepository,
        IFilterParser<Course> filterParser,
        IMapper mapper,
        ILogger<GetCourseListQueryHandler> logger)
    {
        _courseRepository = courseRepository;
        _filterParser = filterParser;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<Result<PaginatedList<CourseDto>>> Handle(
        GetCourseListQuery request,
        CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation(
                "Course list requested. PageNumber: {PageNumber}, PageSize: {PageSize}, Filter: {Filter}",
                request.PageNumber,
                request.PageSize,
                request.Filter ?? "None");

            // ✅ SPECIFICATION PATTERN
            var specification = new CourseFilteredSpecification(
                filterString: request.Filter,
                pageNumber: request.PageNumber,
                pageSize: request.PageSize,
                filterParser: _filterParser);

            var courses = await _courseRepository.ListAsync(specification, cancellationToken);

            var totalCount = await _courseRepository.CountAsync(specification, cancellationToken);

            _logger.LogInformation(
                "Courses retrieved. TotalCount: {TotalCount}, Returned: {ReturnedCount}",
                totalCount,
                courses.Count);

            // DTO Mapping
            var courseDtos = _mapper.Map<List<CourseDto>>(courses);

            var paginatedList = new PaginatedList<CourseDto>(
                courseDtos,
                totalCount,
                request.PageNumber,
                request.PageSize);

            return Result<PaginatedList<CourseDto>>.Success(paginatedList);
        }
        catch (FilterParsingException ex)
        {
            _logger.LogWarning(ex, "Invalid filter format: {Filter}", request.Filter);
            return Result<PaginatedList<CourseDto>>.Failure(
                $"Geçersiz filter formatı: {ex.Message}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving course list");
            return Result<PaginatedList<CourseDto>>.Failure(
                "Ders listesi alınırken bir hata oluştu.");
        }
    }
}