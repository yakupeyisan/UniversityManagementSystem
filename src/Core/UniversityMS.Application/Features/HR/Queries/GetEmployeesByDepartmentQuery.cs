using MediatR;
using UniversityMS.Application.Common.Models;
using UniversityMS.Application.Features.HR.DTOs;

namespace UniversityMS.Application.Features.HR.Queries;

public record GetEmployeesByDepartmentQuery(Guid DepartmentId) : IRequest<Result<List<EmployeeListDto>>>;