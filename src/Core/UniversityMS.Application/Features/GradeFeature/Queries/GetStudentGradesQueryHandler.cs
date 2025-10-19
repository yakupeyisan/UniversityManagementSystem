using System.Linq.Expressions;
using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using UniversityMS.Application.Common.Extensions;
using UniversityMS.Application.Common.Models;
using UniversityMS.Application.Features.GradeFeature.DTOs;
using UniversityMS.Domain.Entities.EnrollmentAggregate;
using UniversityMS.Domain.Interfaces;

namespace UniversityMS.Application.Features.GradeFeature.Queries;

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

            return Result<List<GradeDto>>.Success(gradeDtos);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving student grades. StudentId: {StudentId}", request.StudentId);
            return Result<List<GradeDto>>.Failure("Öğrenci notları alınırken bir hata oluştu.");
        }
    }
}