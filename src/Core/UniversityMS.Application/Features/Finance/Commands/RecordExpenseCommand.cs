using MediatR;
using UniversityMS.Application.Common.Models;

namespace UniversityMS.Application.Features.Finance.Commands;

public record RecordExpenseCommand(
    Guid DepartmentId,
    string Category,
    decimal Amount,
    string Description,
    DateTime ExpenseDate
) : IRequest<Result<ExpenseDto>>;