using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using UniversityMS.Application.Common.Interfaces;
using UniversityMS.Application.Common.Models;
using UniversityMS.Domain.Entities.HRAggregate;
using UniversityMS.Domain.Entities.PayrollAggregate;
using UniversityMS.Domain.Interfaces;

namespace UniversityMS.Application.Features.PayrollFeature.DTOs;

public class CreatePayslipCommandHandler : IRequestHandler<CreatePayslipCommand, Result<PayslipDto>>
{
    private readonly IRepository<Payroll> _payrollRepository;
    private readonly IRepository<Employee> _employeeRepository;
    private readonly IRepository<Payslip> _payslipRepository;
    private readonly IPayslipGenerationService _payslipService;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly ILogger<CreatePayslipCommandHandler> _logger;

    public CreatePayslipCommandHandler(
        IRepository<Payroll> payrollRepository,
        IRepository<Employee> employeeRepository,
        IRepository<Payslip> payslipRepository,
        IPayslipGenerationService payslipService,
        IUnitOfWork unitOfWork,
        IMapper mapper,
        ILogger<CreatePayslipCommandHandler> logger)
    {
        _payrollRepository = payrollRepository;
        _employeeRepository = employeeRepository;
        _payslipRepository = payslipRepository;
        _payslipService = payslipService;
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<Result<PayslipDto>> Handle(
        CreatePayslipCommand request,
        CancellationToken cancellationToken)
    {
        try
        {
            // Bordroyu getir
            var payroll = await _payrollRepository.GetByIdAsync(request.PayrollId, cancellationToken);
            if (payroll == null)
                return Result<PayslipDto>.Failure("Bordro bulunamadı");

            // Çalışanı getir
            var employee = await _employeeRepository.GetByIdAsync(payroll.EmployeeId, cancellationToken);
            if (employee == null)
                return Result<PayslipDto>.Failure("Çalışan bulunamadı");

            // Payslip oluştur
            var payslip = await _payslipService.GeneratePayslipAsync(payroll, employee, cancellationToken);

            // Database'e kaydet
            await _payslipRepository.AddAsync(payslip, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Payslip oluşturuldu: {PayslipId}", payslip.Id);

            var dto = _mapper.Map<PayslipDto>(payslip);
            return Result<PayslipDto>.Success(dto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Payslip oluşturma hatası");
            return Result<PayslipDto>.Failure($"Hata: {ex.Message}");
        }
    }
}