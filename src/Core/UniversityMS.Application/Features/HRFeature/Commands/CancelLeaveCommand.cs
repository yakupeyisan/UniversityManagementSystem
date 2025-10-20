using MediatR;
using UniversityMS.Application.Common.Models;
using UniversityMS.Application.Features.HRFeature.DTOs;

namespace UniversityMS.Application.Features.HRFeature.Commands;

/// <summary>
/// Onaylanmış izin talebini iptal et
/// </summary>
public record CancelLeaveCommand(
    Guid LeaveId,
    string CancellationReason
) : IRequest<Result<LeaveDetailDto>>;