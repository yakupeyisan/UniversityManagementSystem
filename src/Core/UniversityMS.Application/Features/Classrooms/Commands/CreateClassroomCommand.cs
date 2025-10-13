using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UniversityMS.Application.Common.Models;
using UniversityMS.Domain.Enums;

namespace UniversityMS.Application.Features.Classrooms.Commands;

public record CreateClassroomCommand(
    string Code,
    string Name,
    int Capacity,
    ClassroomType Type,
    string? Building = null,
    int? Floor = null,
    bool HasProjector = false,
    bool HasSmartBoard = false,
    bool HasComputers = false,
    bool HasAirConditioning = false,
    int? ComputerCount = null,
    string? Description = null
) : IRequest<Result<Guid>>;

