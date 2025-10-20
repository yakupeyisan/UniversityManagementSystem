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
/// İzin talebini onayla Handler
/// </summary>
public class ApproveLeaveCommandHandler : IRequestHandler<ApproveLeaveCommand, Result<LeaveDetailDto>>
{
    private readonly IRepository<Leave> _leaveRepository;
    private readonly IRepository<Employee> _employeeRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly ICurrentUserService _currentUserService;
    private readonly ILogger<ApproveLeaveCommandHandler> _logger;

    public ApproveLeaveCommandHandler(
        IRepository<Leave> leaveRepository,
        IRepository<Employee> employeeRepository,
        IUnitOfWork unitOfWork,
        IMapper mapper,
        ICurrentUserService currentUserService,
        ILogger<ApproveLeaveCommandHandler> logger)
    {
        _leaveRepository = leaveRepository;
        _employeeRepository = employeeRepository;
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _currentUserService = currentUserService;
        _logger = logger;
    }

    public async Task<Result<LeaveDetailDto>> Handle(
        ApproveLeaveCommand request,
        CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("İzin onaylanıyor. LeaveId: {LeaveId}", request.LeaveId);

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
                    "İzin onaya uygun değil. LeaveId: {LeaveId}, Status: {Status}",
                    request.LeaveId, leave.Status);

                return Result<LeaveDetailDto>.Failure(
                    $"Sadece Bekleme durumundaki izinler onaylanabilir. Mevcut durum: {leave.Status}");
            }

            // ========== 3. ÇALIŞAN BİLGİSİ ALMA ==========
            var employee = await _employeeRepository.GetByIdAsync(leave.EmployeeId, cancellationToken);

            if (employee == null)
            {
                _logger.LogError("Çalışan bulunamadı. EmployeeId: {EmployeeId}", leave.EmployeeId);
                return Result<LeaveDetailDto>.Failure("İzin sahibi çalışan bulunamadı.");
            }

            // ========== 4. ONAYLAYICı BİLGİSİ ALMA ==========
            var approverId = _currentUserService.UserId;

            if (approverId == Guid.Empty)
            {
                _logger.LogError("Onaylayıcı bilgisi alınamadı");
                return Result<LeaveDetailDto>.Failure("Onaylayıcı bilgisi alınamadı.");
            }

            // ========== 5. İZİN ONAYLAMA ==========
            leave.Approve(approverId, request.ApprovalNotes);

            // ========== 6. ÇALIŞAN ÜZERİNDEN ONAYLA ==========
            employee.ApproveLeave(leave.Id, approverId);

            _logger.LogInformation(
                "İzin onaylandı. LeaveId: {LeaveId}, ApproverId: {ApproverId}",
                request.LeaveId, approverId);

            // ========== 7. VERITABANINA KAYDETME ==========
            await _leaveRepository.UpdateAsync(leave, cancellationToken);
            await _employeeRepository.UpdateAsync(employee, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("İzin onayı başarıyla kaydedildi. LeaveId: {LeaveId}", request.LeaveId);

            // ========== 8. DTO HAZIRLAMA VE DÖNÜŞ ==========
            var dto = _mapper.Map<LeaveDetailDto>(leave);

            return Result<LeaveDetailDto>.Success(
                dto,
                "İzin talebi başarıyla onaylandı.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "İzin onaylama hatası. LeaveId: {LeaveId}", request.LeaveId);
            return Result<LeaveDetailDto>.Failure($"İzin onaylama hatası: {ex.Message}");
        }
    }
}
