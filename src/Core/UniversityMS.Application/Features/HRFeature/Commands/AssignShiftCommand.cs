using MediatR;
using UniversityMS.Application.Common.Models;

namespace UniversityMS.Application.Features.HRFeature.Commands;

public record AssignShiftCommand(
    Guid EmployeeId,
    DateOnly Date,
    TimeOnly StartTime,
    TimeOnly EndTime,
    string ShiftPattern,
    string? Notes = null
) : IRequest<Result<Guid>>;