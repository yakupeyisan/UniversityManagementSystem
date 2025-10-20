using MediatR;
using Microsoft.Extensions.Logging;
using UniversityMS.Application.Common.Interfaces;
using UniversityMS.Application.Common.Models;
using UniversityMS.Application.Features.PayrollFeature.DTOs;
using UniversityMS.Domain.Entities.PayrollAggregate;
using UniversityMS.Domain.Interfaces;

namespace UniversityMS.Application.Features.PayrollFeature.Commands;

/// <summary>
/// SGK Hesaplama Handler
/// Türkiye SGK (Sosyal Güvenlik Kurumu) primlerini hesaplar
/// </summary>
public class CalculateSGKCommandHandler : IRequestHandler<CalculateSGKCommand, Result<SGKCalculationResultDto>>
{
    private readonly IRepository<Payroll> _payrollRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ISGKCalculationService _sgkService;  // ➕ EKLE
    private readonly ILogger<CalculateSGKCommandHandler> _logger;

    public CalculateSGKCommandHandler(
        IRepository<Payroll> payrollRepository,
        IUnitOfWork unitOfWork,
        ISGKCalculationService sgkService,  // ➕ EKLE
        ILogger<CalculateSGKCommandHandler> logger)
    {
        _payrollRepository = payrollRepository;
        _unitOfWork = unitOfWork;
        _sgkService = sgkService;  // ➕ EKLE
        _logger = logger;
    }

    public async Task<Result<SGKCalculationResultDto>> Handle(
        CalculateSGKCommand request,
        CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation(
                "SGK hesaplama başlıyor. PayrollId: {PayrollId}", request.PayrollId);

            var sgkResult = await _sgkService.CalculateSGKAsync(
                grossSalary: request.GrossSalary,
                premiumDays: request.PremiumDays,
                isInsured: request.IsInsured);

            _logger.LogInformation(
                "SGK hesaplama tamamlandı. Çalışan: {Emp}, İşveren: {Employer}",
                sgkResult.TotalEmployeeContribution,
                sgkResult.TotalEmployerContribution);

            return Result<SGKCalculationResultDto>.Success(
                sgkResult,
                "SGK hesaplama başarıyla tamamlandı.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "SGK hesaplama hatası");
            return Result<SGKCalculationResultDto>.Failure($"Hata: {ex.Message}");
        }
    }
}