using MediatR;
using UniversityMS.Application.Common.Models;
using UniversityMS.Application.Features.Grades.DTOs;

namespace UniversityMS.Application.Features.Grades.Queries;

public record GetStudentGradesQuery(
    Guid StudentId,
    Guid? CourseId = null
) : IRequest<Result<List<GradeDto>>>;

// ===== GetCourseGradesQuery =====

// ===== GetGradeStatisticsQuery =====