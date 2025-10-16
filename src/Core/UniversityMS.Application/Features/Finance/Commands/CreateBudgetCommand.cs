using MediatR;
using UniversityMS.Application.Common.Models;
using UniversityMS.Application.Features.Finance.DTOs.UniversityMS.Application.Features.Finance.DTOs;

namespace UniversityMS.Application.Features.Finance.Commands;

public record CreateBudgetCommand(
    Guid DepartmentId,
    int Year,
    string BudgetType,
    decimal TotalAmount,
    string Description
) : IRequest<Result<BudgetDto>>;
