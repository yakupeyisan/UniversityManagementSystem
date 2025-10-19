using MediatR;
using UniversityMS.Application.Common.Models;
using UniversityMS.Application.Features.AttendanceFeature.DTOs;

namespace UniversityMS.Application.Features.AttendanceFeature.Queries;


// ===== GetStudentAttendanceQuery =====
public record GetStudentAttendanceQuery(
    Guid StudentId,
    Guid CourseId
) : IRequest<Result<StudentAttendanceDto>>;

// ===== GetAttendanceReportQuery =====