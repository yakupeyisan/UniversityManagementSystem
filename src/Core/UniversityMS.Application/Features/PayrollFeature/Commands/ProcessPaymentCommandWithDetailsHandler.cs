using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using UniversityMS.Application.Common.Models;
using UniversityMS.Application.Features.PayrollFeature.DTOs;
using UniversityMS.Domain.Entities.PayrollAggregate;
using UniversityMS.Domain.Enums;
using UniversityMS.Domain.Interfaces;

namespace UniversityMS.Application.Features.PayrollFeature.Commands;

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