using MediatR;
using UniversityMS.Application.Common.Models;

namespace UniversityMS.Application.Features.ClassroomFeature.Commands;

public record UpdateClassroomCommand(
    Guid Id,
    string Name,
    int Capacity,
    string? Building = null,
    int? Floor = null,
    bool HasProjector = false,
    bool HasSmartBoard = false,
    bool HasComputers = false,
    bool HasAirConditioning = false,
    int? ComputerCount = null,
    string? Description = null
) : IRequest<Result>;