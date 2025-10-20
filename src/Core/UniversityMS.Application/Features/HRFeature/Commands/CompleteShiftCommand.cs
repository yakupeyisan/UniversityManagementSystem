using MediatR;
using UniversityMS.Application.Common.Models;

namespace UniversityMS.Application.Features.HRFeature.Commands;

public record CompleteShiftCommand(
    Guid ShiftId,
    decimal? OvertimeHours = null
) : IRequest<Result<Unit>>;