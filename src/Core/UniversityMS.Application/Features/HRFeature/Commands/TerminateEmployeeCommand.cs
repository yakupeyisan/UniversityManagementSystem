using MediatR;
using UniversityMS.Application.Common.Models;

namespace UniversityMS.Application.Features.HRFeature.Commands;

public record TerminateEmployeeCommand(
    Guid EmployeeId,
    DateTime TerminationDate
) : IRequest<Result<Unit>>;