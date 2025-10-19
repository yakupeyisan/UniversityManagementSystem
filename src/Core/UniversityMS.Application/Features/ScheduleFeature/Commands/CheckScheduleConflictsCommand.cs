using MediatR;
using UniversityMS.Application.Common.Models;
using UniversityMS.Application.Features.ScheduleFeature.DTOs;

namespace UniversityMS.Application.Features.ScheduleFeature.Commands;

public record CheckScheduleConflictsCommand(
    Guid ScheduleId,
    Guid CourseId,
    Guid? InstructorId,
    Guid ClassroomId,
    int DayOfWeek,
    string StartTime,
    string EndTime
) : IRequest<Result<List<ScheduleConflictDto>>>;