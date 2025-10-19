using MediatR;
using UniversityMS.Application.Common.Models;
using UniversityMS.Application.Features.AttendanceFeature.DTOs;

namespace UniversityMS.Application.Features.AttendanceFeature.Commands;


public record TakeAttendanceCommand(
    Guid CourseId,
    Guid InstructorId,
    DateTime AttendanceDate,
    int WeekNumber,
    List<AttendanceDto> Attendances
) : IRequest<Result>;