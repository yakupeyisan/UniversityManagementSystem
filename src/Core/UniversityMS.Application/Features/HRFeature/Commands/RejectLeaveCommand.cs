using MediatR;
using UniversityMS.Application.Common.Models;
using UniversityMS.Application.Features.HRFeature.DTOs;

namespace UniversityMS.Application.Features.HRFeature.Commands;

/// <summary>
/// İzin talebini reddet
/// </summary>
public record RejectLeaveCommand(
    Guid LeaveId,
    string RejectionReason
) : IRequest<Result<LeaveDetailDto>>;
