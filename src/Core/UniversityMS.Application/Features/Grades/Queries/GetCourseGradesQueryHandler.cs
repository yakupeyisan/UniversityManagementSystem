using System.Linq.Expressions;
using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using UniversityMS.Application.Common.Models;
using UniversityMS.Application.Features.Grades.DTOs;
using UniversityMS.Domain.Entities.EnrollmentAggregate;
using UniversityMS.Domain.Interfaces;

namespace UniversityMS.Application.Features.Grades.Queries;

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