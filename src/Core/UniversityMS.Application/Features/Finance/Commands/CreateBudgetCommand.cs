using MediatR;
using UniversityMS.Application.Common.Models;

namespace UniversityMS.Application.Features.Finance.Commands;

public record CreateBudgetCommand(
    Guid DepartmentId,
    int Year,
    string BudgetType,
    decimal TotalAmount,
    string Description
) : IRequest<Result<BudgetDto>>;
