using MediatR;
using UniversityMS.Application.Common.Models;

namespace UniversityMS.Application.Features.Schedules.Commands;


public record CreateScheduleCommand(
    string AcademicYear,
    int Semester,
    string Name,
    string? Description,
    DateTime StartDate,
    DateTime EndDate,
    Guid? DepartmentId = null
) : IRequest<Result<Guid>>;