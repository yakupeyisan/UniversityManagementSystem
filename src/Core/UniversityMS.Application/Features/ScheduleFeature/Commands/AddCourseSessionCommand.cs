using MediatR;
using UniversityMS.Application.Common.Models;
using UniversityMS.Domain.Enums;

namespace UniversityMS.Application.Features.ScheduleFeature.Commands;

public record AddCourseSessionCommand(
    Guid ScheduleId,
    Guid CourseId,
    Guid? InstructorId,
    Guid ClassroomId,
    DayOfWeekEnum DayOfWeek,
    string StartTime,  // "09:00"
    string EndTime,    // "10:50"
    SessionType SessionType,
    string? Notes = null
) : IRequest<Result<Guid>>;