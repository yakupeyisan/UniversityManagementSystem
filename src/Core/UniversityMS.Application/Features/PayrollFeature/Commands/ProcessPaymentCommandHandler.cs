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

/// <summary>
/// Bordro ödemesini işleyen handler
/// Approved → Paid durumuna geçişi yönetir
/// </summary>
public class ProcessPaymentCommandHandler : IRequestHandler<ProcessPaymentCommand, Result<PayrollDto>>
{
    private readonly IRepository<Payroll> _payrollRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly ILogger<ProcessPaymentCommandHandler> _logger;

    public ProcessPaymentCommandHandler(
        IRepository<Payroll> payrollRepository,
        IUnitOfWork unitOfWork,
        IMapper mapper,
        ILogger<ProcessPaymentCommandHandler> logger)
    {
        _payrollRepository = payrollRepository;
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _logger = logger;
    }

    /// <summary>
    /// Ödeme işlemini handle eder
    /// </summary>
    public async Task<Result<PayrollDto>> Handle(
        ProcessPaymentCommand request,
        CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("Ödeme işleme başlıyor. PayrollId: {PayrollId}", request.PayrollId);

            // ========== 1. BORDRO BULMA ==========
            var payroll = await _payrollRepository.GetByIdAsync(request.PayrollId, cancellationToken);

            if (payroll == null)
            {
                _logger.LogWarning("Bordro bulunamadı. PayrollId: {PayrollId}", request.PayrollId);
                return Result<PayrollDto>.Failure("Bordro bulunamadı.");
            }

            _logger.LogInformation(
                "Bordro bulundu. Durum: {Status}, Çalışan: {EmployeeId}",
                payroll.Status,
                payroll.EmployeeId);

            // ========== 2. DURUM KONTROLÜ ==========
            // ✅ FIX: Sadece Approved durumda ödeme yapılabilir (Pending yok)
            if (payroll.Status != PayrollStatus.Approved)
            {
                _logger.LogWarning(
                    "Ödeme işlemi yapılamıyor. Bordro durumu: {Status} (PayrollId: {PayrollId})",
                    payroll.Status,
                    request.PayrollId);

                return Result<PayrollDto>.Failure(
                    $"Sadece Approved durumundaki bordrodan ödeme yapılabilir. " +
                    $"Bordro durumu: {payroll.Status}");
            }

            // ========== 3. ÖNCEKİ ÖDEME KONTROLÜ ==========
            if (payroll.Status == PayrollStatus.Paid)
            {
                _logger.LogWarning(
                    "Bordro zaten ödendi. PayrollId: {PayrollId}",
                    request.PayrollId);

                return Result<PayrollDto>.Failure("Bu bordro zaten ödendi.");
            }

            // ========== 4. ÖDEME IŞLEMI ==========
            try
            {
                // ✅ FIX: MarkAsPaid(Guid paidBy, string paymentReference) parametrelerini sağla
                var paymentReference = $"PAY-{DateTime.UtcNow:yyyyMMddHHmmss}-{Guid.NewGuid().ToString().Substring(0, 8)}";
                var currentUserId = Guid.NewGuid(); // TODO: Gerçek user ID'sini context'ten al

                payroll.MarkAsPaid(currentUserId, paymentReference);

                _logger.LogInformation(
                    "Bordro Paid olarak işaretlendi. PayrollId: {PayrollId}, Referans: {Reference}",
                    payroll.Id,
                    paymentReference);

                // Repository'yi güncelle
                await _payrollRepository.UpdateAsync(payroll, cancellationToken);

                // Database'e kaydet
                await _unitOfWork.SaveChangesAsync(cancellationToken);

                _logger.LogInformation(
                    "Ödeme işlemi başarılı. PayrollId: {PayrollId}",
                    payroll.Id);
            }
            catch (DomainException ex)
            {
                _logger.LogError(
                    ex,
                    "Domain hatası ödeme işleminde. PayrollId: {PayrollId}",
                    request.PayrollId);

                return Result<PayrollDto>.Failure($"Ödeme işlemi hatası: {ex.Message}");
            }

            // ========== 5. DTO OLUŞTUR ==========
            var payrollDto = _mapper.Map<PayrollDto>(payroll);

            _logger.LogInformation(
                "Ödeme işlemi tamamlandı. PayrollId: {PayrollId}, Durum: {Status}",
                payroll.Id,
                payroll.Status);

            return Result<PayrollDto>.Success(payrollDto, "Ödeme başarıyla işlendi.");
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Ödeme işleminde beklenmeyen hata oluştu. PayrollId: {PayrollId}",
                request.PayrollId);

            return Result<PayrollDto>.Failure(
                $"Ödeme işleminde hata oluştu: {ex.Message}");
        }
    }
}


// ============================================
// PROCESS PAYMENT COMMAND WITH DETAILS - FIXED
// ============================================

/// <summary>
/// Ödeme bilgisi ile bordro ödemesi
/// Banka bilgisi, referans numarası vb. bilgiler içerebilir
/// </summary>
public record ProcessPaymentCommandWithDetails(
    Guid PayrollId,
    Guid? PaidBy = null,
    string? BankName = null,
    string? AccountNumber = null,
    string? TransactionReference = null,
    string? Notes = null
) : IRequest<Result<PayrollDto>>;

/// <summary>
/// Detaylı ödeme işlemini handle eden handler
/// </summary>
public class ProcessPaymentCommandWithDetailsHandler : IRequestHandler<ProcessPaymentCommandWithDetails, Result<PayrollDto>>
{
    private readonly IRepository<Payroll> _payrollRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly ILogger<ProcessPaymentCommandWithDetailsHandler> _logger;

    public ProcessPaymentCommandWithDetailsHandler(
        IRepository<Payroll> payrollRepository,
        IUnitOfWork unitOfWork,
        IMapper mapper,
        ILogger<ProcessPaymentCommandWithDetailsHandler> logger)
    {
        _payrollRepository = payrollRepository;
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<Result<PayrollDto>> Handle(
        ProcessPaymentCommandWithDetails request,
        CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation(
                "Detaylı ödeme işleme başlıyor. PayrollId: {PayrollId}, Bank: {BankName}",
                request.PayrollId,
                request.BankName);

            // ========== 1. BORDRO BULMA ==========
            var payroll = await _payrollRepository.GetByIdAsync(request.PayrollId, cancellationToken);

            if (payroll == null)
            {
                _logger.LogWarning("Bordro bulunamadı. PayrollId: {PayrollId}", request.PayrollId);
                return Result<PayrollDto>.Failure("Bordro bulunamadı.");
            }

            // ========== 2. DURUM KONTROLÜ ==========
            // ✅ FIX: Sadece Approved durumda ödeme yapılabilir
            if (payroll.Status != PayrollStatus.Approved)
            {
                _logger.LogWarning("Bordro {Status} durumda. PayrollId: {PayrollId}", payroll.Status, request.PayrollId);
                return Result<PayrollDto>.Failure(
                    $"Ödeme yapılamıyor. Bordro durumu: {payroll.Status}");
            }

            if (payroll.Status == PayrollStatus.Paid)
            {
                _logger.LogWarning("Bordro zaten ödendi. PayrollId: {PayrollId}", request.PayrollId);
                return Result<PayrollDto>.Failure("Bu bordro zaten ödendi.");
            }

            // ========== 3. ÖDEME REFERANSI OLUŞTUR ==========
            // ✅ FIX: TransactionReference'i oluştur veya request'ten al
            var paymentReference = request.TransactionReference
                ?? $"PAY-{DateTime.UtcNow:yyyyMMddHHmmss}-{Guid.NewGuid().ToString().Substring(0, 8)}";

            var paidBy = request.PaidBy ?? Guid.NewGuid(); // TODO: Gerçek user ID'sini al

            // ========== 4. ÖDEME İŞLEMİNİ TAMAMLA ==========
            // ✅ FIX: MarkAsPaid gerekli parametreleri sağla
            payroll.MarkAsPaid(paidBy, paymentReference);

            await _payrollRepository.UpdateAsync(payroll, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _logger.LogInformation(
                "Detaylı ödeme işlemi tamamlandı. PayrollId: {PayrollId}, Referans: {Reference}",
                payroll.Id,
                paymentReference);

            var payrollDto = _mapper.Map<PayrollDto>(payroll);
            return Result<PayrollDto>.Success(payrollDto, "Ödeme başarıyla işlendi.");
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Detaylı ödeme işleminde hata. PayrollId: {PayrollId}",
                request.PayrollId);

            return Result<PayrollDto>.Failure($"Ödeme işleminde hata: {ex.Message}");
        }
    }
}

public record ProcessBatchPaymentCommand(
    List<Guid> PayrollIds,
    Guid? ProcessedBy = null,
    string? Notes = null
) : IRequest<Result<BatchPaymentResultDto>>;

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