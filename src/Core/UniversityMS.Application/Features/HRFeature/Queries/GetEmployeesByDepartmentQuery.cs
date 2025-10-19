using MediatR;
using UniversityMS.Application.Common.Models;
using UniversityMS.Application.Features.HRFeature.DTOs;

namespace UniversityMS.Application.Features.HRFeature.Queries;

public record GetEmployeesByDepartmentQuery(Guid DepartmentId) : IRequest<Result<List<EmployeeListDto>>>;