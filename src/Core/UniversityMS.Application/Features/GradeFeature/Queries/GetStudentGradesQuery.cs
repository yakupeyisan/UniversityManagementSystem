using MediatR;
using UniversityMS.Application.Common.Models;
using UniversityMS.Application.Features.GradeFeature.DTOs;

namespace UniversityMS.Application.Features.GradeFeature.Queries;

public record GetStudentGradesQuery(
    Guid StudentId,
    Guid? CourseId = null
) : IRequest<Result<List<GradeDto>>>;

// ===== GetCourseGradesQuery =====

// ===== GetGradeStatisticsQuery =====