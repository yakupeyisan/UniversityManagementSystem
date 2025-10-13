using MediatR;
using UniversityMS.Application.Common.Models;
using UniversityMS.Application.Features.Schedules.DTOs;

namespace UniversityMS.Application.Features.Schedules.Queries;

public record CheckScheduleConflictsCommand(
    Guid ScheduleId,
    Guid CourseId,
    Guid? InstructorId,
    Guid ClassroomId,
    int DayOfWeek,
    string StartTime,
    string EndTime
) : IRequest<Result<ScheduleConflictDto>>;