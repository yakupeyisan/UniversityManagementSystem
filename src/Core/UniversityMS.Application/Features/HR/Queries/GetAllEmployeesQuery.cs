using MediatR;
using UniversityMS.Application.Common.Models;
using UniversityMS.Application.Features.HR.DTOs;

namespace UniversityMS.Application.Features.HR.Queries;

public record GetAllEmployeesQuery(
    int PageNumber = 1,
    int PageSize = 10,
    string? SearchTerm = null,
    Guid? DepartmentId = null,
    string? Status = null
) : IRequest<Result<PaginatedList<EmployeeListDto>>>;