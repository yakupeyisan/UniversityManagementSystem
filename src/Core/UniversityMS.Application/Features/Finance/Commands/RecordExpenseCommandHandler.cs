using AutoMapper;
using MediatR;
using UniversityMS.Application.Common.Models;
using UniversityMS.Application.Features.Finance.DTOs.UniversityMS.Application.Features.Finance.DTOs;
using UniversityMS.Domain.Entities.FinanceAggregate;
using UniversityMS.Domain.Interfaces;
using UniversityMS.Domain.ValueObjects;

namespace UniversityMS.Application.Features.Finance.Commands;

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