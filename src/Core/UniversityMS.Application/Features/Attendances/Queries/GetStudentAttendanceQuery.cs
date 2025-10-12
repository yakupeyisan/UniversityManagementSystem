using MediatR;
using UniversityMS.Application.Common.Models;
using UniversityMS.Application.Features.Attendances.DTOs;

namespace UniversityMS.Application.Features.Attendances.Queries;


// ===== GetStudentAttendanceQuery =====
public record GetStudentAttendanceQuery(
    Guid StudentId,
    Guid CourseId
) : IRequest<Result<StudentAttendanceDto>>;

// ===== GetAttendanceReportQuery =====