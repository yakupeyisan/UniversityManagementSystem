using MediatR;
using UniversityMS.Application.Common.Models;
using UniversityMS.Application.Features.AttendanceFeature.DTOs;

namespace UniversityMS.Application.Features.AttendanceFeature.Queries;

public record GetAttendanceReportQuery(
    Guid CourseId,
    DateTime? StartDate = null,
    DateTime? EndDate = null
) : IRequest<Result<AttendanceReportDto>>;