using MediatR;
using UniversityMS.Application.Common.Interfaces;
using UniversityMS.Application.Common.Models;
using UniversityMS.Application.Features.PayrollFeature.DTOs;
using UniversityMS.Domain.Entities.PayrollAggregate;
using UniversityMS.Domain.Interfaces;

namespace UniversityMS.Application.Features.PayrollFeature.Commands;

public class GeneratePayslipCommandHandler : IRequestHandler<GeneratePayslipCommand, Result<PayslipDto>>
{
    private readonly IRepository<Payroll> _payrollRepository;
    private readonly IPayslipGenerationService _payslipService;
    private readonly IUnitOfWork _unitOfWork;

    public GeneratePayslipCommandHandler(
        IRepository<Payroll> payrollRepository,
        IPayslipGenerationService payslipService,
        IUnitOfWork unitOfWork)
    {
        _payrollRepository = payrollRepository;
        _payslipService = payslipService;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<PayslipDto>> Handle(
        GeneratePayslipCommand request,
        CancellationToken cancellationToken)
    {
        var payroll = await _payrollRepository.GetByIdAsync(request.PayrollId, cancellationToken);
        if (payroll == null)
            return Result<PayslipDto>.Failure("Bordro bulunamadı");
        var payslip = await _payslipService.GeneratePayslipAsync(
            payroll,
            payroll.Employee,
            cancellationToken);

        return Result<PayslipDto>.Success(new PayslipDto
        {
            Id = payslip.Id,
            PayrollId = payslip.PayrollId,
            EmployeeId = payslip.EmployeeId,
            Month = payslip.Month.ToString(),
            Year = payslip.Year,
            FilePath = payslip.FilePath,
            Status = payslip.Status.ToString()
        });
    }
}