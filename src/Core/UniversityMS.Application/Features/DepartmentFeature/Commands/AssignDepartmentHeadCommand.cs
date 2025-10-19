using MediatR;
using UniversityMS.Application.Common.Models;

namespace UniversityMS.Application.Features.DepartmentFeature.Commands;

public record AssignDepartmentHeadCommand(
    Guid DepartmentId,
    Guid FacultyId
) : IRequest<Result>;