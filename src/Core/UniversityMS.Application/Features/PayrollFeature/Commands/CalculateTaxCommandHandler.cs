using MediatR;
using Microsoft.Extensions.Logging;
using UniversityMS.Application.Common.Interfaces;
using UniversityMS.Application.Common.Models;
using UniversityMS.Application.Features.PayrollFeature.DTOs;
using UniversityMS.Domain.Entities.PayrollAggregate;
using UniversityMS.Domain.Interfaces;

namespace UniversityMS.Application.Features.PayrollFeature.Commands;

/// <summary>
/// Gelir Vergisi Hesaplama Handler
/// Türkiye 2025 gelir vergisi tarifesini kullanır
/// </summary>
public class CalculateTaxCommandHandler : IRequestHandler<CalculateTaxCommand, Result<TaxCalculationResultDto>>
{
    private readonly IRepository<Payroll> _payrollRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ITaxCalculationService _taxService;  // ➕ EKLE
    private readonly ILogger<CalculateTaxCommandHandler> _logger;

    public CalculateTaxCommandHandler(
        IRepository<Payroll> payrollRepository,
        IUnitOfWork unitOfWork,
        ITaxCalculationService taxService,  // ➕ EKLE
        ILogger<CalculateTaxCommandHandler> logger)
    {
        _payrollRepository = payrollRepository;
        _unitOfWork = unitOfWork;
        _taxService = taxService;  // ➕ EKLE
        _logger = logger;
    }

    public async Task<Result<TaxCalculationResultDto>> Handle(
        CalculateTaxCommand request,
        CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation(
                "Vergi hesaplama başlıyor. PayrollId: {PayrollId}", request.PayrollId);

            var taxResult = await _taxService.CalculateTaxDetailedAsync(
                grossSalary: request.GrossSalary,
                sgkDeductions: request.SGKDeductions,
                taxDiscount: request.TaxDiscount,
                taxYear: request.TaxYear,
                taxMonth: request.TaxMonth);

            _logger.LogInformation(
                "Vergi hesaplama tamamlandı. Vergi: {Tax}", taxResult.IncomeTax);

            return Result<TaxCalculationResultDto>.Success(
                taxResult,
                "Vergi hesaplama başarıyla tamamlandı.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Vergi hesaplama hatası");
            return Result<TaxCalculationResultDto>.Failure($"Hata: {ex.Message}");
        }
    }
}