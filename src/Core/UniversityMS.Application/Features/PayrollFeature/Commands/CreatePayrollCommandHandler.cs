using AutoMapper;
using MediatR;
using UniversityMS.Application.Common.Models;
using UniversityMS.Application.Features.PayrollFeature.DTOs;
using UniversityMS.Domain.Entities.HRAggregate;
using UniversityMS.Domain.Entities.PayrollAggregate;
using UniversityMS.Domain.Enums;
using UniversityMS.Domain.Interfaces;
using UniversityMS.Domain.ValueObjects;

namespace UniversityMS.Application.Features.PayrollFeature.Commands;

public class CreatePayrollCommandHandler : IRequestHandler<CreatePayrollCommand, Result<PayrollDto>>
{
    private readonly IRepository<Payroll> _payrollRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CreatePayrollCommandHandler(
        IRepository<Payroll> payrollRepository,
        IUnitOfWork unitOfWork)
    {
        _payrollRepository = payrollRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<PayrollDto>> Handle(
        CreatePayrollCommand request,
        CancellationToken cancellationToken)
    {
        var payrollNumber = $"PR-{request.Year}-{request.Month:D2}-{Guid.NewGuid().ToString().Substring(0, 8)}";
        var baseSalary = Money.Create(request.BaseSalary, "TRY");
        var paymentMethod = Enum.Parse<PaymentMethod>(request.PaymentMethod);

        var payroll = Payroll.Create(
            payrollNumber,
            request.EmployeeId,
            request.Year,
            request.Month,
            baseSalary,
            request.WorkingDays,
            paymentMethod
        );

        if (request.ActualWorkDays > 0)
            payroll.SetActualWorkDays(request.ActualWorkDays);
        payroll.SetOvertimeHours(request.OvertimeHours);

        await _payrollRepository.AddAsync(payroll, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result<PayrollDto>.Success(new PayrollDto
        {
            Id = payroll.Id,
            PayrollNumber = payroll.PayrollNumber,
            Month = payroll.Month,
            Year = payroll.Year,
            Status = payroll.Status.ToString(),
            BaseSalary = payroll.BaseSalary.Amount
        });
    }
}
