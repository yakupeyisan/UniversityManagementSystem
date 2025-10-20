using MediatR;
using UniversityMS.Application.Common.Models;
using UniversityMS.Application.Features.HRFeature.DTOs;

namespace UniversityMS.Application.Features.HRFeature.Commands;

/// <summary>
/// İzin talebini onayla
/// </summary>
public record ApproveLeaveCommand(
    Guid LeaveId,
    string? ApprovalNotes = null
) : IRequest<Result<LeaveDetailDto>>;