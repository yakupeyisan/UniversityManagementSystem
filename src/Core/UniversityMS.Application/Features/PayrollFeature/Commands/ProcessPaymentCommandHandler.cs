using AutoMapper;
using MediatR;
using UniversityMS.Application.Common.Models;
using UniversityMS.Application.Features.PayrollFeature.DTOs;
using UniversityMS.Domain.Entities.PayrollAggregate;
using UniversityMS.Domain.Interfaces;

namespace UniversityMS.Application.Features.PayrollFeature.Commands;

public class ProcessPaymentCommandHandler : IRequestHandler<ProcessPaymentCommand, Result<PayrollDto>>
{
    private readonly IRepository<Payroll> _payrollRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public ProcessPaymentCommandHandler(
        IRepository<Payroll> payrollRepository,
        IUnitOfWork unitOfWork,
        IMapper mapper)
    {
        _payrollRepository = payrollRepository;
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<Result<PayrollDto>> Handle(
        ProcessPaymentCommand request,
        CancellationToken cancellationToken)
    {
        var payroll = await _payrollRepository.GetByIdAsync(request.PayrollId, cancellationToken);
        if (payroll is null)
            return Result<PayrollDto>.Failure("Bordro bulunamadı");

        if (payroll.Status.ToString() != "Approved")
            return Result<PayrollDto>.Failure("Sadece onaylı bordrolar işlenebilir");

        payroll.ProcessPayment(request.PaymentMethod, request.BankAccountNumber);

        await _payrollRepository.UpdateAsync(payroll, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        var dto = _mapper.Map<PayrollDto>(payroll);
        return Result<PayrollDto>.Success(dto);
    }
}