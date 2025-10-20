using MediatR;
using UniversityMS.Application.Common.Models;

namespace UniversityMS.Application.Features.HRFeature.Commands;

public record CancelLeaveCommand(Guid LeaveId) : IRequest<Result<Unit>>;