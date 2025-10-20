using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using UniversityMS.Application.Common.Models;
using UniversityMS.Application.Features.FinanceFeature.DTOs;
using UniversityMS.Domain.Entities.FinanceAggregate;
using UniversityMS.Domain.Interfaces;

namespace UniversityMS.Application.Features.FinanceFeature.Queries;

public class GetBudgetByIdQueryHandler : IRequestHandler<GetBudgetByIdQuery, Result<BudgetDto>>
{
    private readonly IRepository<Budget> _repository;
    private readonly IMapper _mapper;
    private readonly ILogger<GetBudgetByIdQueryHandler> _logger;

    public GetBudgetByIdQueryHandler(
        IRepository<Budget> repository,
        IMapper mapper,
        ILogger<GetBudgetByIdQueryHandler> logger)
    {
        _repository = repository;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<Result<BudgetDto>> Handle(
        GetBudgetByIdQuery request,
        CancellationToken cancellationToken)
    {
        try
        {
            if (request.BudgetId == Guid.Empty)
                return Result<BudgetDto>.Failure("Geçersiz bütçe ID'si.");

            var budget = await _repository.GetByIdAsync(request.BudgetId, cancellationToken);
            if (budget == null)
                return Result<BudgetDto>.Failure("Bütçe bulunamadı.");

            var dto = _mapper.Map<BudgetDto>(budget);
            _logger.LogInformation("Retrieved budget: {BudgetId}", request.BudgetId);
            return Result<BudgetDto>.Success(dto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving budget {BudgetId}", request.BudgetId);
            return Result<BudgetDto>.Failure(
                $"Bütçe alınırken hata oluştu: {ex.Message}");
        }
    }
}