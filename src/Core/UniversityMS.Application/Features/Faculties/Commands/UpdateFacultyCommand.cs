using MediatR;
using UniversityMS.Application.Common.Models;

namespace UniversityMS.Application.Features.Faculties.Commands;

public record UpdateFacultyCommand(
    Guid Id,
    string Name,
    string Code,
    string? Description
) : IRequest<Result<Guid>>;