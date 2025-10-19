using MediatR;
using UniversityMS.Application.Common.Models;

namespace UniversityMS.Application.Features.FacultyFeature.Commands;

public record CreateFacultyCommand(
    string Name,
    string Code,
    string? Description = null
) : IRequest<Result<Guid>>;