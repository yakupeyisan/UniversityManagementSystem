using MediatR;
using UniversityMS.Application.Common.Models;
using UniversityMS.Application.Features.Schedules.DTOs;

namespace UniversityMS.Application.Features.Classrooms.Queries;

public record GetClassroomScheduleQuery(
    Guid ClassroomId,
    string AcademicYear,
    int Semester
) : IRequest<Result<WeeklyScheduleDto>>;