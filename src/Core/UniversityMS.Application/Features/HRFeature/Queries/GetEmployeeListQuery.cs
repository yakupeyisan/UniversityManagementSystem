using MediatR;
using UniversityMS.Application.Common.Models;
using UniversityMS.Application.Features.HRFeature.DTOs;

namespace UniversityMS.Application.Features.HRFeature.Queries;

public record GetEmployeeListQuery(
    int PageNumber = 1,
    int PageSize = 10,
    Guid? DepartmentId = null,
    string? Status = null,
    string? SearchTerm = null
) : IRequest<Result<PaginatedList<EmployeeDto>>>;