using MediatR;
using UniversityMS.Application.Common.Models;
using UniversityMS.Application.Features.ScheduleFeature.DTOs;

namespace UniversityMS.Application.Features.ScheduleFeature.Queries;

public record GetWeeklyScheduleQuery(
    Guid ScheduleId,
    Guid? DepartmentId = null,
    Guid? InstructorId = null,
    Guid? StudentId = null
) : IRequest<Result<WeeklyScheduleDto>>;
