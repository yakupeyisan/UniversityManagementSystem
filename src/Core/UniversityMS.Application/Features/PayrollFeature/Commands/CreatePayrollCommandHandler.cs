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

        // ✅ DÜZELTILMIŞ: Constructor'a doğru argümanlar geç
        var payroll = Payroll.Create(
            payrollNumber: payrollNumber,
            employeeId: request.EmployeeId,
            year: request.Year,
            month: request.Month,
            baseSalary: baseSalary,
            workingDays: payrollPeriod.GetWorkingDays(),
            paymentMethod: PaymentMethod.BankTransfer,
            bankAccount: null
        );

        // ✅ DÜZELTILMIŞ: Factory method ile PayrollDeduction oluştur
        if (request.Deductions != null && request.Deductions.Any())
        {
            foreach (var deduction in request.Deductions)
            {
                var deductionAmount = Money.Create(deduction.Amount, "TRY");

                // PayrollDeduction.Create() factory method'u kullan
                var payrollDeduction = PayrollDeduction.Create(
                    payrollId: payroll.Id,
                    type: Enum.Parse<DeductionType>(deduction.Type),
                    description: deduction.Description ?? deduction.Type,
                    amount: deductionAmount,
                    rate: deduction.Rate,
                    isStatutory: false
                );

                payroll.AddDeduction(payrollDeduction);
            }
        }

        // ✅ DÜZELTILMIŞ: AddEarning yerine AddItem() kullan (Bonus varsa)
        if (request.Bonus.HasValue && request.Bonus > 0)
        {
            var bonusAmount = Money.Create(request.Bonus.Value, "TRY");

            // PayrollItem.Create() factory method'u kullan
            var bonusItem = PayrollItem.Create(
                payrollId: payroll.Id,
                type: PayrollItemType.Earning,
                category: "Bonus",
                description: "Bonus Ödeneği",
                amount: bonusAmount,
                quantity: null,
                isTaxable: true
            );

            payroll.AddItem(bonusItem);
        }

        await _payrollRepository.AddAsync(payroll, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        var dto = _mapper.Map<PayrollDto>(payroll);
        return Result<PayrollDto>.Success(dto);
    }
}