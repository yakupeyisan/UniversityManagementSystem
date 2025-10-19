using MediatR;
using UniversityMS.Application.Common.Models;
using UniversityMS.Application.Features.StaffFeature.DTOs;

namespace UniversityMS.Application.Features.StaffFeature.Queries;

public record SearchEmployeesQuery(
    string? SearchTerm = null,
    Guid? DepartmentId = null,
    string? Position = null,
    int PageNumber = 1,
    int PageSize = 10
) : IRequest<Result<PaginatedList<EmployeeDto>>>;
