using MediatR;
using UniversityMS.Application.Common.Models;
using UniversityMS.Application.Features.Attendances.DTOs;

namespace UniversityMS.Application.Features.Attendances.Queries;

public record GetAttendanceReportQuery(
    Guid CourseId,
    DateTime? StartDate = null,
    DateTime? EndDate = null
) : IRequest<Result<AttendanceReportDto>>;