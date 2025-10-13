using MediatR;
using UniversityMS.Application.Common.Models;
using UniversityMS.Application.Features.Schedules.DTOs;

namespace UniversityMS.Application.Features.Schedules.Queries;

public record GetWeeklyScheduleQuery(
    Guid ScheduleId,
    Guid? DepartmentId = null,
    Guid? InstructorId = null,
    Guid? StudentId = null
) : IRequest<Result<WeeklyScheduleDto>>;
