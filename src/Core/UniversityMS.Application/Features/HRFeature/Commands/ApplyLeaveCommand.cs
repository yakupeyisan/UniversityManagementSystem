using MediatR;
using UniversityMS.Application.Common.Models;
using UniversityMS.Application.Features.HRFeature.DTOs;

namespace UniversityMS.Application.Features.HRFeature.Commands;

/// <summary>
/// İzin başvurusu command record'ı
/// </summary>
public record ApplyLeaveCommand(
    Guid EmployeeId,
    int LeaveTypeId,
    DateTime StartDate,
    DateTime EndDate,
    string Reason
) : IRequest<Result<LeaveRequestDto>>;