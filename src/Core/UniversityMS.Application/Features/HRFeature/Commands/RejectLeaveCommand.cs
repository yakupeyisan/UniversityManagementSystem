using MediatR;
using UniversityMS.Application.Common.Models;

namespace UniversityMS.Application.Features.HRFeature.Commands;

public record RejectLeaveCommand(
    Guid LeaveId,
    Guid RejectingUserId,
    string Reason
) : IRequest<Result<Unit>>;