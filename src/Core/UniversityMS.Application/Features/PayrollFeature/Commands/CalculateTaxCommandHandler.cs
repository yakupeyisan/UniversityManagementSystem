using MediatR;
using Microsoft.Extensions.Logging;
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
    private readonly ILogger<CalculateTaxCommandHandler> _logger;

    public CalculateTaxCommandHandler(
        IRepository<Payroll> payrollRepository,
        IUnitOfWork unitOfWork,
        ILogger<CalculateTaxCommandHandler> logger)
    {
        _payrollRepository = payrollRepository;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Result<TaxCalculationResultDto>> Handle(
        CalculateTaxCommand request,
        CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation(
                "Vergi hesaplama başlıyor. PayrollId: {PayrollId}, Brüt: {Gross}",
                request.PayrollId, request.GrossSalary);

            // ========== VALIDASYON ==========
            if (request.GrossSalary <= 0)
                return Result<TaxCalculationResultDto>.Failure("Brüt maaş sıfırdan büyük olmalıdır.");

            // ========== VERİ HAZIRLAMA ==========
            var taxYear = request.TaxYear ?? DateTime.UtcNow.Year;
            var taxMonth = request.TaxMonth ?? DateTime.UtcNow.Month;
            var grossSalary = request.GrossSalary;
            var sgkDeductions = request.SGKDeductions;

            // ========== VERGİLENDİRİLEBİLİR GELİR (Brüt - SGK) ==========
            var taxableIncome = grossSalary - sgkDeductions;

            if (taxableIncome <= 0)
                taxableIncome = grossSalary; // SGK yoksa brüt üzerinden hesapla

            // ========== 2025 GELİR VERGİSİ TARİFESİ (TÜRKIYE) ==========
            // Asgari Ücret: ~39.000 TL (2025 örnek)
            // Vergi Matrahı Sınırları:
            // 0 - 11.000 TL: %0 (Muaf)
            // 11.001 - 30.000 TL: %15
            // 30.001 - 110.000 TL: %20
            // 110.001 - 240.000 TL: %27
            // 240.001+ TL: %32

            decimal incomeTax = 0m;
            decimal incomeTaxRate = 0m;

            if (taxableIncome <= 11000)
            {
                incomeTax = 0;
                incomeTaxRate = 0;
            }
            else if (taxableIncome <= 30000)
            {
                incomeTax = (taxableIncome - 11000) * 0.15m;
                incomeTaxRate = 15;
            }
            else if (taxableIncome <= 110000)
            {
                incomeTax = (19000 * 0.15m) + ((taxableIncome - 30000) * 0.20m);
                incomeTaxRate = 20;
            }
            else if (taxableIncome <= 240000)
            {
                incomeTax = (19000 * 0.15m) + (80000 * 0.20m) + ((taxableIncome - 110000) * 0.27m);
                incomeTaxRate = 27;
            }
            else
            {
                incomeTax = (19000 * 0.15m) + (80000 * 0.20m) + (130000 * 0.27m) + ((taxableIncome - 240000) * 0.32m);
                incomeTaxRate = 32;
            }

            // ========== DAMGA VERGİSİ (%0.759) ==========
            // Damga vergisi brüt maaş üzerinden hesaplanır
            decimal stampDutyRate = 0.00759m;
            decimal stampDuty = grossSalary * stampDutyRate;

            // ========== STOPAJ VERGİSİ (Varsa - Ek İşi vb.) ==========
            decimal withholdingTax = 0m;
            decimal withholdingTaxRate = 0m;
            // Stopaj normalde bordro dışında ek gelirler için geçerlidir

            // ========== GEÇİCİ VERGİ STOPAJI (Varsa) ==========
            decimal temporaryTaxDeduction = 0m;

            // ========== VERGİ İNDİRİMİ (Varsa) ==========
            decimal taxDiscount = 0m;
            if (request.TaxDiscount.HasValue && request.TaxDiscount.Value > 0)
            {
                taxDiscount = (incomeTax + stampDuty) * (request.TaxDiscount.Value / 100m);
                _logger.LogInformation(
                    "Vergi indirimi uygulandı: {Rate}%, Tutar: {Amount}",
                    request.TaxDiscount.Value, taxDiscount);
            }

            // ========== TOPLAM HESAPLAMA ==========
            incomeTax = Math.Max(0, incomeTax - taxDiscount);
            decimal totalTaxDeductions = incomeTax + stampDuty + withholdingTax + temporaryTaxDeduction;
            decimal netAmountAfterTax = grossSalary - totalTaxDeductions;

            // ========== SONUÇ HAZIRLAMA ==========
            var result = new TaxCalculationResultDto
            {
                GrossSalary = grossSalary,
                SGKDeductions = sgkDeductions,
                TaxableIncome = taxableIncome,
                AdjustedTaxableIncome = taxableIncome,
                IncomeIncomeTaxRate = incomeTaxRate,
                IncomeTax = incomeTax,
                IncomeTaxDiscount = request.TaxDiscount,
                AfterIncomeTax = grossSalary - incomeTax,
                WithholdingTaxRate = withholdingTaxRate,
                WithholdingTax = withholdingTax,
                StampDutyRate = stampDutyRate * 100,
                StampDuty = stampDuty,
                TemporaryTaxDeduction = temporaryTaxDeduction,
                TotalTaxDeductions = totalTaxDeductions,
                NetAmountAfterTax = netAmountAfterTax,
                TaxYear = taxYear,
                TaxMonth = taxMonth,
                TaxTableVersion = "2025-Turkey-v1",
                CalculatedDate = DateTime.UtcNow,
                CalculatedBy = "PayrollSystem",
                CalculationNotes = $"Vergi hesaplama dönemi: {taxMonth}/{taxYear}. Vergi oranı: %{incomeTaxRate}",
                ApplicableLaws = "Gelir Vergisi Kanunu - Gümrük ve Ticaret Bakanlığı 2025"
            };

            _logger.LogInformation(
                "Vergi hesaplama tamamlandı. PayrollId: {PayrollId}, Vergi: {Tax}, Net: {Net}",
                request.PayrollId, incomeTax, netAmountAfterTax);

            return Result<TaxCalculationResultDto>.Success(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Vergi hesaplama hatası. PayrollId: {PayrollId}", request.PayrollId);
            return Result<TaxCalculationResultDto>.Failure($"Vergi hesaplama hatası: {ex.Message}");
        }
    }
}