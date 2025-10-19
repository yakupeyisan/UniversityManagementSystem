using MediatR;
using UniversityMS.Application.Common.Models;
using UniversityMS.Application.Features.FinanceFeature.DTOs;

namespace UniversityMS.Application.Features.FinanceFeature.Commands;

public record RecordExpenseCommand(
    Guid DepartmentId,
    Guid? BudgetItemId,
    string Category,
    decimal Amount,
    string Description,
    DateTime ExpenseDate
) : IRequest<Result<ExpenseDto>>;