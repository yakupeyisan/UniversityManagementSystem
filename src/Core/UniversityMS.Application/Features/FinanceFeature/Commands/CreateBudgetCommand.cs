using MediatR;
using UniversityMS.Application.Common.Models;
using UniversityMS.Application.Features.FinanceFeature.DTOs;

namespace UniversityMS.Application.Features.FinanceFeature.Commands;

public record CreateBudgetCommand(
    Guid DepartmentId,
    int Year,
    string BudgetType,
    decimal TotalAmount,
    string Description
) : IRequest<Result<BudgetDto>>;
