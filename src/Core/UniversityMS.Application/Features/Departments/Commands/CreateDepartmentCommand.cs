using MediatR;
using UniversityMS.Application.Common.Models;

namespace UniversityMS.Application.Features.Departments.Commands;


public record CreateDepartmentCommand(
    string Name,
    string Code,
    Guid FacultyId,
    string? Description
) : IRequest<Result<Guid>>;