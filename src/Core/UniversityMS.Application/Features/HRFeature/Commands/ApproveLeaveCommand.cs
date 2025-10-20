using MediatR;
using UniversityMS.Application.Common.Models;

namespace UniversityMS.Application.Features.HRFeature.Commands;

public record ApproveLeaveCommand(
    Guid LeaveId,
    Guid ApproverId
) : IRequest<Result<Unit>>;