using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using UniversityMS.Application.Common.Interfaces;
using UniversityMS.Application.Common.Models;
using UniversityMS.Application.Features.PayrollFeature.DTOs;
using UniversityMS.Domain.Entities.PayrollAggregate;
using UniversityMS.Domain.Enums;
using UniversityMS.Domain.Exceptions;
using UniversityMS.Domain.Interfaces;

namespace UniversityMS.Application.Features.PayrollFeature.Commands;


public class GeneratePayslipCommandHandler : IRequestHandler<GeneratePayslipCommand, Result<PayslipDto>>
{
    private readonly IRepository<Payroll> _payrollRepository;
    private readonly IRepository<Payslip> _payslipRepository;
    private readonly IPayslipGenerationService _payslipGenerationService;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<GeneratePayslipCommandHandler> _logger;

    public GeneratePayslipCommandHandler(
        IRepository<Payroll> payrollRepository,
        IRepository<Payslip> payslipRepository,
        IPayslipGenerationService payslipGenerationService,
        IUnitOfWork unitOfWork,
        ILogger<GeneratePayslipCommandHandler> logger)
    {
        _payrollRepository = payrollRepository;
        _payslipRepository = payslipRepository;
        _payslipGenerationService = payslipGenerationService;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Result<PayslipDto>> Handle(
        GeneratePayslipCommand request,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Generating payslip for payroll {PayrollId}", request.PayrollId);

        try
        {
            // Bordro bul
            var payroll = await _payrollRepository.GetByIdAsync(request.PayrollId, cancellationToken);

            if (payroll == null)
            {
                _logger.LogWarning("Payroll not found: {PayrollId}", request.PayrollId);
                return Result<PayslipDto>.Failure("Bordro bulunamadı.");
            }

            // ✅ Durum kontrolü: Sadece onaylı bordrodan payslip yap
            if (payroll.Status != PayrollStatus.Approved)
            {
                _logger.LogWarning(
                    "Cannot generate payslip for payroll {PayrollId} with status {Status}",
                    request.PayrollId,
                    payroll.Status);

                return Result<PayslipDto>.Failure(
                    "Sadece onaylı bordrodan payslip oluşturulabilir. " +
                    $"Bordro durumu: {payroll.Status}");
            }

            // Payslip oluştur (service çağrı)
            _logger.LogInformation(
                "Calling payslip generation service for employee {EmployeeId}",
                payroll.EmployeeId);

            var payslip = await _payslipGenerationService.GeneratePayslipAsync(
                payroll,
                payroll.Employee,
                cancellationToken);

            // ✅ KRITIK: Payslip'i repository'ye ekle
            await _payslipRepository.AddAsync(payslip, cancellationToken);

            _logger.LogInformation("Payslip added to repository: {PayslipId}", payslip.Id);

            // ✅ KRITIK: Bordro statusu güncelle
            payroll.GeneratePayslip(payslip.Id);

            _logger.LogInformation("Payroll status updated: {PayrollId}", payroll.Id);

            // ✅ KRITIK: Database'e kaydet
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Changes saved to database for payslip {PayslipId}", payslip.Id);

            // DTO oluştur ve döndür
            var payslipDto = MapPayslipToDto(payslip, payroll);

            _logger.LogInformation(
                "Payslip generated successfully. PayslipId: {PayslipId}, EmployeeId: {EmployeeId}",
                payslip.Id,
                payslip.EmployeeId);

            return Result<PayslipDto>.Success(payslipDto);
        }
        catch (DomainException ex)
        {
            _logger.LogError(ex, "Domain error while generating payslip");
            return Result<PayslipDto>.Failure(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error while generating payslip");
            return Result<PayslipDto>.Failure("Payslip oluşturulurken hata oluştu: " + ex.Message);
        }
    }

    /// <summary>
    /// Payslip'i DTO'ya mapla
    /// </summary>
    private PayslipDto MapPayslipToDto(Payslip payslip, Payroll payroll)
    {
        return new PayslipDto
        {
            Id = payslip.Id,
            PayrollId = payslip.PayrollId,
            EmployeeId = payslip.EmployeeId,
            PayrollNumber = payslip.PayrollNumber,
            Year = payslip.Year,
            Month = payslip.Month,
            Period = payslip.Period,
            EmployeeName = payroll.Employee.Person.FullName,
            EmployeeNumber = payroll.Employee.EmployeeNumber.Value,
            Department = payroll.Employee.Department?.Name ?? "N/A",
            BaseSalary = payroll.BaseSalary.Amount,
            GrossSalary = payslip.GrossSalary.Amount,
            TotalEarnings = payroll.TotalEarnings.Amount,
            TotalDeductions = payslip.TotalDeductions.Amount,
            NetSalary = payslip.NetSalary.Amount,
            IncomeTax = payslip.IncomeTax?.Amount ?? 0,
            SGKEmployeeContribution = payslip.SGKEmployeeContribution?.Amount ?? 0,
            FilePath = payslip.FilePath ?? string.Empty,
            Status = payslip.Status.ToString(),
            GeneratedDate = payslip.GeneratedDate ?? DateTime.UtcNow,
            Earnings = new List<PayslipLineItemDto>(),
            Deductions = new List<PayslipLineItemDto>()
        };
    }
}