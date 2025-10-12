using MediatR;
using UniversityMS.Application.Common.Models;
using UniversityMS.Application.Features.Departments.DTOs;

namespace UniversityMS.Application.Features.Departments.Queries;

public record GetDepartmentByIdQuery(Guid Id) : IRequest<Result<DepartmentDto>>;