using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using UniversityMS.Application.Common.Models;
using UniversityMS.Application.Features.FinanceFeature.DTOs;
using UniversityMS.Domain.Entities.FinanceAggregate;
using UniversityMS.Domain.Interfaces;
using UniversityMS.Domain.Specifications;

namespace UniversityMS.Application.Features.FinanceFeature.Queries;

public class GetBudgetListQueryHandler : IRequestHandler<GetBudgetListQuery, Result<PaginatedList<BudgetDto>>>
{
    private readonly IRepository<Budget> _repository;
    private readonly IMapper _mapper;
    private readonly ILogger<GetBudgetListQueryHandler> _logger;

    public GetBudgetListQueryHandler(
        IRepository<Budget> repository,
        IMapper mapper,
        ILogger<GetBudgetListQueryHandler> logger)
    {
        _repository = repository;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<Result<PaginatedList<BudgetDto>>> Handle(
        GetBudgetListQuery request,
        CancellationToken cancellationToken)
    {
        try
        {
            var spec = new BudgetFilteredSpecification(
                request.FiscalYear,
                request.DepartmentId,
                request.Status,
                request.PageNumber,
                request.PageSize);

            var budgets = await _repository.ListAsync(spec, cancellationToken);
            var total = await _repository.CountAsync(spec, cancellationToken);
            var dtos = _mapper.Map<List<BudgetDto>>(budgets);

            var paginated = new PaginatedList<BudgetDto>(
                dtos, total, request.PageNumber, request.PageSize);

            _logger.LogInformation("Retrieved {Count} budgets for fiscal year {Year}",
                budgets.Count, request.FiscalYear);
            return Result<PaginatedList<BudgetDto>>.Success(paginated);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving budgets");
            return Result<PaginatedList<BudgetDto>>.Failure(
                $"Bütçeler alınırken hata oluştu: {ex.Message}");
        }
    }
}