using MediatR;
using UniversityMS.Application.Common.Models;
using UniversityMS.Application.Features.ScheduleFeature.DTOs;
using UniversityMS.Domain.Enums;

namespace UniversityMS.Application.Features.ClassroomFeature.Queries;

public record FindAvailableClassroomsQuery(
    string AcademicYear,
    int Semester,
    DayOfWeekEnum DayOfWeek,
    string StartTime,
    string EndTime,
    int RequiredCapacity,
    ClassroomType? PreferredType = null,
    bool NeedsProjector = false,
    bool NeedsComputers = false
) : IRequest<Result<List<ClassroomDto>>>;