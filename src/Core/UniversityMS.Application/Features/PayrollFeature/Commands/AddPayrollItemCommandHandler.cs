using AutoMapper;
using MediatR;
using UniversityMS.Application.Common.Models;
using UniversityMS.Application.Features.PayrollFeature.DTOs;
using UniversityMS.Domain.Entities.PayrollAggregate;
using UniversityMS.Domain.Enums;
using UniversityMS.Domain.Interfaces;
using UniversityMS.Domain.ValueObjects;

namespace UniversityMS.Application.Features.PayrollFeature.Commands;

public class AddPayrollItemCommandHandler : IRequestHandler<AddPayrollItemCommand, Result<PayrollDto>>
{
    private readonly IRepository<Payroll> _payrollRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public AddPayrollItemCommandHandler(
        IRepository<Payroll> payrollRepository,
        IUnitOfWork unitOfWork,
        IMapper mapper)
    {
        _payrollRepository = payrollRepository;
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<Result<PayrollDto>> Handle(
        AddPayrollItemCommand request,
        CancellationToken cancellationToken)
    {
        var payroll = await _payrollRepository.GetByIdAsync(request.PayrollId, cancellationToken);
        if (payroll is null)
            return Result<PayrollDto>.Failure("Bordro bulunamadı");

        var amount = Money.Create(request.Amount, "TRY");

        var item = PayrollItem.Create(
            payrollId: payroll.Id,
            type: Enum.Parse<PayrollItemType>(request.Type),
            category: request.Category,
            description: request.Description,
            amount: amount,
            quantity: request.Quantity,
            isTaxable: request.IsTaxable
        );

        payroll.AddItem(item);

        await _payrollRepository.UpdateAsync(payroll, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        var dto = _mapper.Map<PayrollDto>(payroll);
        return Result<PayrollDto>.Success(dto);
    }
}