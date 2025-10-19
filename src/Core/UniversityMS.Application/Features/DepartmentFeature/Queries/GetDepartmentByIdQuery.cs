using MediatR;
using UniversityMS.Application.Common.Models;
using UniversityMS.Application.Features.DepartmentFeature.DTOs;

namespace UniversityMS.Application.Features.DepartmentFeature.Queries;

public record GetDepartmentByIdQuery(Guid Id) : IRequest<Result<DepartmentDto>>;