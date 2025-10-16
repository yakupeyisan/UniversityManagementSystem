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
    private readonly IRepository<Employee> _employeeRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public CreatePayrollCommandHandler(
        IRepository<Payroll> payrollRepository,
        IRepository<Employee> employeeRepository,
        IUnitOfWork unitOfWork,
        IMapper mapper)
    {
        _payrollRepository = payrollRepository;
        _employeeRepository = employeeRepository;
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<Result<PayrollDto>> Handle(
        CreatePayrollCommand request,
        CancellationToken cancellationToken)
    {
        // Çalışan kontrol
        var employee = await _employeeRepository.GetByIdAsync(request.EmployeeId, cancellationToken);
        if (employee is null)
            return Result<PayrollDto>.Failure("Çalışan bulunamadı");

        // Duplicate kontrol
        var existingPayroll = await _payrollRepository.GetAllAsync(cancellationToken);
        if (existingPayroll.Any(p =>
                p.EmployeeId == request.EmployeeId &&
                p.Year == request.Year &&
                p.Month == request.Month))
            return Result<PayrollDto>.Failure("Bu ay için bordro zaten oluşturulmuş");

        // Value Objects oluştur
        var payrollPeriod = PayrollPeriod.Create(request.Year, request.Month);
        var baseSalary = Money.Create(request.BaseSalary, "TRY");

        string payrollNumber = $"PAY-{request.Year}{request.Month:D2}-{request.EmployeeId.ToString().Substring(0, 8).ToUpper()}";

        // Payroll oluştur
        var payroll = new Payroll(
            payrollNumber,
            request.EmployeeId,
            request.Year,
            request.Month,
            baseSalary,
            payrollPeriod.GetWorkingDays(),
            PaymentMethod.BankTransfer
        );

        // Kesintileri ekle
        foreach (var deduction in request.Deductions ?? new List<PayrollDeductionDto>())
        {
            var deductionAmount = Money.Create(deduction.Amount, "TRY");
            var payrollDeduction = new PayrollDeduction(
                deduction.Type,
                deductionAmount,
                deduction.Description
            );
            payroll.AddDeduction(payrollDeduction);
        }

        // Bonus varsa ekle
        if (request.Bonus.HasValue && request.Bonus > 0)
        {
            var bonusAmount = Money.Create(request.Bonus.Value, "TRY");
            payroll.AddEarning(bonusAmount, "Bonus");
        }

        await _payrollRepository.AddAsync(payroll, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        var dto = _mapper.Map<PayrollDto>(payroll);
        return Result<PayrollDto>.Success(dto);
    }
}