using MediatR;
using Microsoft.Extensions.Logging;
using UniversityMS.Application.Common.Interfaces;
using UniversityMS.Application.Common.Models;
using UniversityMS.Domain.Entities.HRAggregate;
using UniversityMS.Domain.Entities.PayrollAggregate;
using UniversityMS.Domain.Interfaces;

namespace UniversityMS.Application.Features.PayrollFeature.DTOs;

public class SendPayslipEmailCommandHandler : IRequestHandler<SendPayslipEmailCommand, Result<bool>>
{
    private readonly IRepository<Payslip> _payslipRepository;
    private readonly IRepository<Employee> _employeeRepository;
    private readonly IPayslipGenerationService _payslipService;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<SendPayslipEmailCommandHandler> _logger;

    public SendPayslipEmailCommandHandler(
        IRepository<Payslip> payslipRepository,
        IRepository<Employee> employeeRepository,
        IPayslipGenerationService payslipService,
        IUnitOfWork unitOfWork,
        ILogger<SendPayslipEmailCommandHandler> logger)
    {
        _payslipRepository = payslipRepository;
        _employeeRepository = employeeRepository;
        _payslipService = payslipService;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Result<bool>> Handle(
        SendPayslipEmailCommand request,
        CancellationToken cancellationToken)
    {
        try
        {
            var payslip = await _payslipRepository.GetByIdAsync(request.PayslipId, cancellationToken);
            if (payslip == null)
                return Result<bool>.Failure("Payslip bulunamadı");

            var employee = await _employeeRepository.GetByIdAsync(payslip.EmployeeId, cancellationToken);
            if (employee == null)
                return Result<bool>.Failure("Çalışan bulunamadı");

            var result = await _payslipService.SendPayslipEmailAsync(payslip, employee, cancellationToken);

            if (result)
            {
                await _unitOfWork.SaveChangesAsync(cancellationToken);
                _logger.LogInformation("Payslip email gönderildi: {PayslipId}", payslip.Id);
            }

            return Result<bool>.Success(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Email gönderme hatası");
            return Result<bool>.Failure($"Hata: {ex.Message}");
        }
    }
}