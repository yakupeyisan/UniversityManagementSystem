using MediatR;
using UniversityMS.Application.Common.Models;
using UniversityMS.Application.Features.HRFeature.DTOs;

namespace UniversityMS.Application.Features.HRFeature.Queries;

public record GetActiveEmployeesQuery(int PageNumber = 1, int PageSize = 10)
    : IRequest<Result<PaginatedList<EmployeeListDto>>>;