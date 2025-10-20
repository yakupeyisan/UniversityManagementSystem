using MediatR;
using UniversityMS.Application.Common.Models;
using UniversityMS.Domain.Enums;

namespace UniversityMS.Application.Features.ScheduleFeature.Commands;

public record UpdateScheduleCommand(
    Guid Id,
    string Name,
    string? Description,
    DateTime StartDate,
    DateTime EndDate,
    ScheduleStatus Status
) : IRequest<Result<Guid>>;