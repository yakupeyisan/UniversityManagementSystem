using MediatR;
using UniversityMS.Application.Common.Models;
using UniversityMS.Application.Features.HR.DTOs;

namespace UniversityMS.Application.Features.HR.Queries;

public record GetActiveEmployeesQuery(int PageNumber = 1, int PageSize = 10)
    : IRequest<Result<PaginatedList<EmployeeListDto>>>;