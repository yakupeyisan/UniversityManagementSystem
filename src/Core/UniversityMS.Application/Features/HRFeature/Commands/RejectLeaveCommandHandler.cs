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
/// İzin talebini reddet Handler
/// </summary>
public class RejectLeaveCommandHandler : IRequestHandler<RejectLeaveCommand, Result<LeaveDetailDto>>
{
    private readonly IRepository<Leave> _leaveRepository;
    private readonly IRepository<Employee> _employeeRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly ICurrentUserService _currentUserService;
    private readonly ILogger<RejectLeaveCommandHandler> _logger;

    public RejectLeaveCommandHandler(
        IRepository<Leave> leaveRepository,
        IRepository<Employee> employeeRepository,
        IUnitOfWork unitOfWork,
        IMapper mapper,
        ICurrentUserService currentUserService,
        ILogger<RejectLeaveCommandHandler> logger)
    {
        _leaveRepository = leaveRepository;
        _employeeRepository = employeeRepository;
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _currentUserService = currentUserService;
        _logger = logger;
    }

    public async Task<Result<LeaveDetailDto>> Handle(
        RejectLeaveCommand request,
        CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation(
                "İzin reddediliyor. LeaveId: {LeaveId}, Reason: {Reason}",
                request.LeaveId, request.RejectionReason);

            // ========== 1. İZİN BULMA ==========
            var leave = await _leaveRepository.GetByIdAsync(request.LeaveId, cancellationToken);

            if (leave == null)
            {
                _logger.LogWarning("İzin bulunamadı. LeaveId: {LeaveId}", request.LeaveId);
                return Result<LeaveDetailDto>.Failure("İzin talebi bulunamadı.");
            }

            // ========== 2. DURUM KONTROLÜ ==========
            if (leave.Status.ToString() != "Pending")
            {
                _logger.LogWarning(
                    "İzin reddetmeye uygun değil. LeaveId: {LeaveId}, Status: {Status}",
                    request.LeaveId, leave.Status);

                return Result<LeaveDetailDto>.Failure(
                    $"Sadece Bekleme durumundaki izinler reddedilebilir.");
            }

            // ========== 3. REDDEDİCİ BİLGİSİ ALMA ==========
            var rejectorId = _currentUserService.UserId;

            if (rejectorId == Guid.Empty)
            {
                _logger.LogError("Reddeden kullanıcı bilgisi alınamadı");
                return Result<LeaveDetailDto>.Failure("Kullanıcı bilgisi alınamadı.");
            }

            // ========== 4. ÇALIŞAN BİLGİSİ ALMA ==========
            var employee = await _employeeRepository.GetByIdAsync(leave.EmployeeId, cancellationToken);

            if (employee == null)
            {
                _logger.LogError("Çalışan bulunamadı. EmployeeId: {EmployeeId}", leave.EmployeeId);
                return Result<LeaveDetailDto>.Failure("İzin sahibi çalışan bulunamadı.");
            }

            // ========== 5. İZİN REDDETME (Entity metodu) ==========
            leave.Reject(rejectorId, request.RejectionReason);

            // ========== 6. ÇALIŞAN ÜZERİNDEN İZİN İADE ETME ==========
            // İzin reddedildiğinde, kullanılan izin günleri geri iade edilir
            employee.RejectLeave(leave.Id, rejectorId, request.RejectionReason);

            _logger.LogInformation(
                "İzin reddedildi. LeaveId: {LeaveId}, RejectorId: {RejectorId}",
                request.LeaveId, rejectorId);

            // ========== 7. VERITABANINA KAYDETME ==========
            await _leaveRepository.UpdateAsync(leave, cancellationToken);
            await _employeeRepository.UpdateAsync(employee, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("İzin reddi başarıyla kaydedildi. LeaveId: {LeaveId}", request.LeaveId);

            // ========== 8. DTO HAZIRLAMA VE DÖNÜŞ ==========
            var dto = _mapper.Map<LeaveDetailDto>(leave);

            return Result<LeaveDetailDto>.Success(
                dto,
                $"İzin talebi başarıyla reddedildi. Neden: {request.RejectionReason}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "İzin reddetme hatası. LeaveId: {LeaveId}", request.LeaveId);
            return Result<LeaveDetailDto>.Failure($"İzin reddetme hatası: {ex.Message}");
        }
    }
}
