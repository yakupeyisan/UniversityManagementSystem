using AutoMapper;
using MediatR;
using UniversityMS.Application.Common.Models;
using UniversityMS.Application.Features.FinanceFeature.DTOs;
using UniversityMS.Domain.Entities.FinanceAggregate;
using UniversityMS.Domain.Enums;
using UniversityMS.Domain.Interfaces;
using UniversityMS.Domain.ValueObjects;

namespace UniversityMS.Application.Features.FinanceFeature.Commands;

public class CreateBudgetCommandHandler : IRequestHandler<CreateBudgetCommand, Result<BudgetDto>>
{
    private readonly IRepository<Budget> _budgetRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public CreateBudgetCommandHandler(
        IRepository<Budget> budgetRepository,
        IUnitOfWork unitOfWork,
        IMapper mapper)
    {
        _budgetRepository = budgetRepository;
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<Result<BudgetDto>> Handle(
        CreateBudgetCommand request,
        CancellationToken cancellationToken)
    {
        // Value Objects oluştur
        var totalAmount = Money.Create(request.TotalAmount, "TRY");

        var budget = Budget.Create(
            budgetCode: $"BUD-{request.Year}-{Guid.NewGuid().ToString().Substring(0, 8)}",
            fiscalYear: request.Year,
            name: $"Budget {request.Year}",  // veya request'ten al
            description: request.Description ?? "Budget description",
            type: Enum.Parse<BudgetType>(request.BudgetType),  // string'den enum'a çevir
            totalAmount: totalAmount,
            startDate: DateTime.UtcNow,
            endDate: DateTime.UtcNow.AddMonths(12),
            departmentId: request.DepartmentId
        );

        await _budgetRepository.AddAsync(budget, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        var dto = _mapper.Map<BudgetDto>(budget);
        return Result<BudgetDto>.Success(dto);
    }
}