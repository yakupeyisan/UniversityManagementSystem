using MediatR;
using UniversityMS.Application.Common.Models;
using UniversityMS.Application.Features.FinanceFeature.DTOs;

namespace UniversityMS.Application.Features.FinanceFeature.Queries;


public record GetBudgetListQuery(
    int PageNumber = 1,
    int PageSize = 10,
    int? FiscalYear = null,
    Guid? DepartmentId = null,
    string? Status = null
) : IRequest<Result<PaginatedList<BudgetDto>>>;