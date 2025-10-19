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