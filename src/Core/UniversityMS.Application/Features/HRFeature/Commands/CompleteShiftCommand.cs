using MediatR;
using UniversityMS.Application.Common.Models;
using UniversityMS.Application.Features.HRFeature.DTOs;

namespace UniversityMS.Application.Features.HRFeature.Commands;

/// <summary>
/// Tamamlanan vardiyayı kayıt et
/// </summary>
public record CompleteShiftCommand(
    Guid ShiftId,
    TimeOnly ActualEndTime,
    string? Notes = null
) : IRequest<Result<ShiftDto>>;