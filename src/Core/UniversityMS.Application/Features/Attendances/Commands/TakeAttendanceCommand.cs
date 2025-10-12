using MediatR;
using UniversityMS.Application.Common.Models;
using UniversityMS.Application.Features.Attendances.DTOs;

namespace UniversityMS.Application.Features.Attendances.Commands;


public record TakeAttendanceCommand(
    Guid CourseId,
    Guid InstructorId,
    DateTime AttendanceDate,
    int WeekNumber,
    List<AttendanceDto> Attendances
) : IRequest<Result<int>>;