using MediatR;
using UniversityMS.Application.Common.Models;
using UniversityMS.Application.Features.ScheduleFeature.DTOs;

namespace UniversityMS.Application.Features.ClassroomFeature.Queries;

public record GetClassroomScheduleQuery(
    Guid ClassroomId,
    string AcademicYear,
    int Semester
) : IRequest<Result<WeeklyScheduleDto>>;