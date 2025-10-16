using AutoMapper;
using MediatR;
using UniversityMS.Application.Common.Models;
using UniversityMS.Domain.Entities.FinanceAggregate;
using UniversityMS.Domain.Interfaces;
using UniversityMS.Domain.ValueObjects;

namespace UniversityMS.Application.Features.Finance.Commands;

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

        var budget = new Budget(
            request.DepartmentId,
            request.Year,
            request.BudgetType,
            totalAmount,
            request.Description
        );

        await _budgetRepository.AddAsync(budget, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        var dto = _mapper.Map<BudgetDto>(budget);
        return Result<BudgetDto>.Success(dto);
    }
}