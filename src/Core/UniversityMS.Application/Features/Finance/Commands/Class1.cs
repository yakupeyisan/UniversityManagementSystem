using AutoMapper;
using FluentValidation;
using MediatR;
using UniversityMS.Application.Common.Models;
using UniversityMS.Domain.Entities.FinanceAggregate;
using UniversityMS.Domain.Interfaces;
using UniversityMS.Domain.ValueObjects;

namespace UniversityMS.Application.Features.Finance.Commands;

public record CreateBudgetCommand(
    Guid DepartmentId,
    int Year,
    string BudgetType,
    decimal TotalAmount,
    string Description
) : IRequest<Result<BudgetDto>>;

// Handler
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

// Validator
public class CreateBudgetCommandValidator : AbstractValidator<CreateBudgetCommand>
{
    public CreateBudgetCommandValidator()
    {
        RuleFor(x => x.DepartmentId).NotEqual(Guid.Empty);
        RuleFor(x => x.Year).GreaterThanOrEqualTo(2020);
        RuleFor(x => x.TotalAmount).GreaterThan(0);
        RuleFor(x => x.BudgetType).NotEmpty().Must(BeValidBudgetType);
    }

    private bool BeValidBudgetType(string type) =>
        new[] { "Personnel", "Operations", "Capital", "Maintenance" }.Contains(type);
}

public record RecordExpenseCommand(
    Guid DepartmentId,
    string Category,
    decimal Amount,
    string Description,
    DateTime ExpenseDate
) : IRequest<Result<ExpenseDto>>;

public class RecordExpenseCommandHandler : IRequestHandler<RecordExpenseCommand, Result<ExpenseDto>>
{
    private readonly IRepository<Transaction> _transactionRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public RecordExpenseCommandHandler(
        IRepository<Transaction> transactionRepository,
        IUnitOfWork unitOfWork,
        IMapper mapper)
    {
        _transactionRepository = transactionRepository;
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<Result<ExpenseDto>> Handle(
        RecordExpenseCommand request,
        CancellationToken cancellationToken)
    {
        // Value Objects oluştur
        var amount = Money.Create(request.Amount, "TRY");
        // Gider negatif money
        var negativeAmount = Money.Create(-request.Amount, "TRY");

        var transaction = new Transaction(
            request.DepartmentId,
            "Expense",
            negativeAmount,
            request.Category,
            request.Description,
            request.ExpenseDate
        );

        await _transactionRepository.AddAsync(transaction, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        var dto = _mapper.Map<ExpenseDto>(transaction);
        return Result<ExpenseDto>.Success(dto);
    }
}
