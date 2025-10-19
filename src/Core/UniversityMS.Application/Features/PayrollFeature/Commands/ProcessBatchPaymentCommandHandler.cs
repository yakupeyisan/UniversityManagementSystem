using MediatR;
using Microsoft.Extensions.Logging;
using UniversityMS.Application.Common.Models;
using UniversityMS.Application.Features.PayrollFeature.DTOs;
using UniversityMS.Domain.Entities.PayrollAggregate;
using UniversityMS.Domain.Enums;
using UniversityMS.Domain.Interfaces;

namespace UniversityMS.Application.Features.PayrollFeature.Commands;

public class ProcessBatchPaymentCommandHandler : IRequestHandler<ProcessBatchPaymentCommand, Result<BatchPaymentResultDto>>
{
    private readonly IRepository<Payroll> _payrollRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<ProcessBatchPaymentCommandHandler> _logger;

    public ProcessBatchPaymentCommandHandler(
        IRepository<Payroll> payrollRepository,
        IUnitOfWork unitOfWork,
        ILogger<ProcessBatchPaymentCommandHandler> logger)
    {
        _payrollRepository = payrollRepository;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Result<BatchPaymentResultDto>> Handle(
        ProcessBatchPaymentCommand request,
        CancellationToken cancellationToken)
    {
        var result = new BatchPaymentResultDto
        {
            TotalProcessed = request.PayrollIds.Count,
            ProcessedDate = DateTime.UtcNow,
            SuccessCount = 0,
            FailureCount = 0,
            TotalAmountPaid = 0
        };

        try
        {
            _logger.LogInformation(
                "Toplu ödeme işleme başlıyor. Toplam bordro: {Count}",
                request.PayrollIds.Count);

            // Transaction başlat
            await _unitOfWork.BeginTransactionAsync(cancellationToken);

            var processedBy = request.ProcessedBy ?? Guid.NewGuid(); // TODO: Gerçek user ID'sini al

            foreach (var payrollId in request.PayrollIds)
            {
                try
                {
                    var payroll = await _payrollRepository.GetByIdAsync(payrollId, cancellationToken);

                    if (payroll == null)
                    {
                        result.FailureCount++;
                        result.Errors.Add($"Bordro {payrollId} bulunamadı.");
                        continue;
                    }

                    // ✅ FIX: Sadece Approved durumda ödeme
                    if (payroll.Status != PayrollStatus.Approved)
                    {
                        result.FailureCount++;
                        result.Errors.Add($"Bordro {payrollId} durumu uygun değil: {payroll.Status}");
                        continue;
                    }

                    // ✅ FIX: MarkAsPaid gerekli parametreleri sağla
                    var paymentReference = $"BATCH-{DateTime.UtcNow:yyyyMMddHHmmss}-{payrollId.ToString().Substring(0, 8)}";
                    payroll.MarkAsPaid(processedBy, paymentReference);

                    await _payrollRepository.UpdateAsync(payroll, cancellationToken);

                    result.SuccessCount++;
                    result.TotalAmountPaid += payroll.NetSalary.Amount;

                    _logger.LogInformation(
                        "Bordro ödendi. PayrollId: {PayrollId}, Tutar: {Amount}",
                        payroll.Id,
                        payroll.NetSalary.Amount);
                }
                catch (Exception ex)
                {
                    result.FailureCount++;
                    result.Errors.Add($"Bordro {payrollId} ödeme hatası: {ex.Message}");
                    _logger.LogError(ex, "Bordro ödeme hatası. PayrollId: {PayrollId}", payrollId);
                }
            }

            // Tüm değişiklikleri kaydet
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            await _unitOfWork.CommitTransactionAsync(cancellationToken);

            _logger.LogInformation(
                "Toplu ödeme tamamlandı. Başarılı: {Success}, Başarısız: {Failure}",
                result.SuccessCount,
                result.FailureCount);

            return Result<BatchPaymentResultDto>.Success(result);
        }
        catch (Exception ex)
        {
            // ✅ FIX: RollbackTransactionAsync() - parametre yok
            try
            {
                await _unitOfWork.RollbackTransactionAsync(cancellationToken);
            }
            catch
            {
                // Rollback başarısız olsa da devam et
            }

            _logger.LogError(
                ex,
                "Toplu ödeme işleminde kritik hata");

            return Result<BatchPaymentResultDto>.Failure(
                $"Toplu ödeme işleminde hata: {ex.Message}");
        }
    }
}