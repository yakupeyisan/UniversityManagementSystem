using MediatR;
using UniversityMS.Application.Common.Models;

namespace UniversityMS.Application.Features.DepartmentFeature.Commands;


public record CreateDepartmentCommand(
    string Name,
    string Code,
    Guid FacultyId,
    string? Description = null
) : IRequest<Result<Guid>>;