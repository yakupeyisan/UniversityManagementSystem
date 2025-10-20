using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using UniversityMS.Application.Common.Interfaces;
using UniversityMS.Application.Common.Models;
using UniversityMS.Application.Features.HRFeature.DTOs;
using UniversityMS.Domain.Entities.HRAggregate;
using UniversityMS.Domain.Interfaces;

namespace UniversityMS.Application.Features.HRFeature.Commands;

/// <summary>
/// Onaylanmış izin talebini iptal et Handler
/// </summary>
public class CancelLeaveCommandHandler : IRequestHandler<CancelLeaveCommand, Result<LeaveDetailDto>>
{
    private readonly IRepository<Leave> _leaveRepository;
    private readonly IRepository<Employee> _employeeRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly ICurrentUserService _currentUserService;
    private readonly ILogger<CancelLeaveCommandHandler> _logger;

    public CancelLeaveCommandHandler(
        IRepository<Leave> leaveRepository,
        IRepository<Employee> employeeRepository,
        IUnitOfWork unitOfWork,
        IMapper mapper,
        ICurrentUserService currentUserService,
        ILogger<CancelLeaveCommandHandler> logger)
    {
        _leaveRepository = leaveRepository;
        _employeeRepository = employeeRepository;
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _currentUserService = currentUserService;
        _logger = logger;
    }

    public async Task<Result<LeaveDetailDto>> Handle(
        CancelLeaveCommand request,
        CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation(
                "İzin iptal edilіyor. LeaveId: {LeaveId}, Reason: {Reason}",
                request.LeaveId, request.CancellationReason);

            // ========== 1. İZİN BULMA ==========
            var leave = await _leaveRepository.GetByIdAsync(request.LeaveId, cancellationToken);

            if (leave == null)
            {
                _logger.LogWarning("İzin bulunamadı. LeaveId: {LeaveId}", request.LeaveId);
                return Result<LeaveDetailDto>.Failure("İzin talebi bulunamadı.");
            }

            // ========== 2. DURUM KONTROLÜ ==========
            // Sadece Onaylanmış (Approved) durumda olan izinler iptal edilebilir
            if (leave.Status.ToString() != "Approved")
            {
                _logger.LogWarning(
                    "İzin iptale uygun değil. LeaveId: {LeaveId}, Status: {Status}",
                    request.LeaveId, leave.Status);

                return Result<LeaveDetailDto>.Failure(
                    $"Sadece Onaylanmış durumundaki izinler iptal edilebilir. Mevcut durum: {leave.Status}");
            }

            // ========== 3. İPTAL EDENİ BİLGİSİ ALMA ==========
            var cancelledById = _currentUserService.UserId;

            if (cancelledById == Guid.Empty)
            {
                _logger.LogError("İptal eden kullanıcı bilgisi alınamadı");
                return Result<LeaveDetailDto>.Failure("Kullanıcı bilgisi alınamadı.");
            }

            // ========== 4. ÇALIŞAN BİLGİSİ ALMA ==========
            var employee = await _employeeRepository.GetByIdAsync(leave.EmployeeId, cancellationToken);

            if (employee == null)
            {
                _logger.LogError("Çalışan bulunamadı. EmployeeId: {EmployeeId}", leave.EmployeeId);
                return Result<LeaveDetailDto>.Failure("İzin sahibi çalışan bulunamadı.");
            }

            // ========== 5. İZİN İPTAL ETME ==========
            leave.Cancel(cancelledById, request.CancellationReason);

            // ========== 6. ÇALIŞAN ÜZERİNDEN İZİN İADE ETME ==========
            // İzin iptal edildiğinde, kullanılan izin günleri geri iade edilir
            var durationDays = (int)(leave.EndDate - leave.StartDate).TotalDays + 1;
            employee.AnnualLeaveBalance = employee.AnnualLeaveBalance.RefundLeave(durationDays);

            _logger.LogInformation(
                "İzin iptal edildi. LeaveId: {LeaveId}, CancelledById: {CancelledById}",
                request.LeaveId, cancelledById);

            // ========== 7. VERITABANINA KAYDETME ==========
            await _leaveRepository.UpdateAsync(leave, cancellationToken);
            await _employeeRepository.UpdateAsync(employee, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("İzin iptali başarıyla kaydedildi. LeaveId: {LeaveId}", request.LeaveId);

            // ========== 8. DTO HAZIRLAMA VE DÖNÜŞ ==========
            var dto = _mapper.Map<LeaveDetailDto>(leave);

            return Result<LeaveDetailDto>.Success(
                dto,
                $"İzin talebi başarıyla iptal edildi. Neden: {request.CancellationReason}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "İzin iptal hatası. LeaveId: {LeaveId}", request.LeaveId);
            return Result<LeaveDetailDto>.Failure($"İzin iptal hatası: {ex.Message}");
        }
    }
}