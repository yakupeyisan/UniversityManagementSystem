using MediatR;
using UniversityMS.Application.Common.Models;
using UniversityMS.Application.Features.HRFeature.DTOs;

namespace UniversityMS.Application.Features.HRFeature.Queries;

public record GetAllEmployeesQuery(
    int PageNumber = 1,
    int PageSize = 10,
    string? Filter = null
) : IRequest<Result<PaginatedList<EmployeeListDto>>>;
