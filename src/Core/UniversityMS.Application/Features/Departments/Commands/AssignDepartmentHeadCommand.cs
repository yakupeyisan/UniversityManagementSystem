using MediatR;
using UniversityMS.Application.Common.Models;

namespace UniversityMS.Application.Features.Departments.Commands;

public record AssignDepartmentHeadCommand(
    Guid DepartmentId,
    Guid FacultyId
) : IRequest<Result>;