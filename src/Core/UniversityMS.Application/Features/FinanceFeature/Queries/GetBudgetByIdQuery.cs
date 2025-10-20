using MediatR;
using UniversityMS.Application.Common.Models;
using UniversityMS.Application.Features.FinanceFeature.DTOs;

namespace UniversityMS.Application.Features.FinanceFeature.Queries;

public record GetBudgetByIdQuery(Guid BudgetId)
    : IRequest<Result<BudgetDto>>;