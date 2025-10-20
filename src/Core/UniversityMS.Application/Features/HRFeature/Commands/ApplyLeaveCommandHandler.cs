using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using UniversityMS.Application.Common.Models;
using UniversityMS.Application.Features.HRFeature.DTOs;
using UniversityMS.Domain.Entities.HRAggregate;
using UniversityMS.Domain.Enums;
using UniversityMS.Domain.Exceptions;
using UniversityMS.Domain.Interfaces;

namespace UniversityMS.Application.Features.HRFeature.Commands;

/// <summary>
/// İzin Talep Handler'ı
/// Çalışanların izin taleplerini işler
/// </summary>
public class ApplyLeaveCommandHandler : IRequestHandler<ApplyLeaveCommand, Result<LeaveRequestDto>>
{
    private readonly IRepository<Employee> _employeeRepository;
    private readonly IRepository<Leave> _leaveRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly ILogger<ApplyLeaveCommandHandler> _logger;

    public ApplyLeaveCommandHandler(
        IRepository<Employee> employeeRepository,
        IRepository<Leave> leaveRepository,
        IUnitOfWork unitOfWork,
        IMapper mapper,
        ILogger<ApplyLeaveCommandHandler> logger)
    {
        _employeeRepository = employeeRepository;
        _leaveRepository = leaveRepository;
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<Result<LeaveRequestDto>> Handle(
        ApplyLeaveCommand request,
        CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation(
                "İzin talebi işleniyor. EmployeeId: {EmployeeId}, LeaveType: {LeaveType}, StartDate: {StartDate}",
                request.EmployeeId, request.LeaveTypeId, request.StartDate);

            // ========== 1. ÇALIŞAN KONTROLÜ ==========
            var employee = await _employeeRepository.GetByIdAsync(request.EmployeeId, cancellationToken);

            if (employee == null)
            {
                _logger.LogWarning("Çalışan bulunamadı. EmployeeId: {EmployeeId}", request.EmployeeId);
                return Result<LeaveRequestDto>.Failure("Çalışan bulunamadı.");
            }

            _logger.LogInformation(
                "Çalışan bulundu: {EmployeeName} (Status: {Status})",
                employee.Person.FirstName + " " + employee.Person.LastName,
                employee.Status);

            // ========== 2. ÇALIŞAN DURUMU KONTROLÜ ==========
            if (employee.Status != EmploymentStatus.Active)
            {
                _logger.LogWarning(
                    "Çalışan aktif değil. EmployeeId: {EmployeeId}, Status: {Status}",
                    request.EmployeeId, employee.Status);

                return Result<LeaveRequestDto>.Failure(
                    $"Sadece aktif çalışanlar izin talep edebilir. Mevcut durum: {employee.Status}");
            }

            // ========== 3. TARİH VALIDASYONU ==========
            if (request.StartDate.Date < DateTime.UtcNow.Date)
            {
                _logger.LogWarning(
                    "Geçmiş tarih için izin talebi. EmployeeId: {EmployeeId}, StartDate: {StartDate}",
                    request.EmployeeId, request.StartDate);

                return Result<LeaveRequestDto>.Failure("İzin başlangıç tarihi geçmiş olamaz.");
            }

            if (request.EndDate < request.StartDate)
            {
                _logger.LogWarning(
                    "Bitiş tarihi başlangıçtan önce. EmployeeId: {EmployeeId}",
                    request.EmployeeId);

                return Result<LeaveRequestDto>.Failure(
                    "İzin bitiş tarihi başlangıç tarihinden sonra olmalıdır.");
            }

            // ========== 4. İZİN TİPİ KONTROLÜ ==========
            var leaveType = (LeaveType)request.LeaveTypeId;
            _logger.LogInformation("İzin tipi: {LeaveType}", leaveType);

            // ========== 5. İZİN BAKIYESI KONTROLÜ (Annual Leave için) ==========
            var durationDays = (request.EndDate.Date - request.StartDate.Date).Days + 1;

            if (leaveType == LeaveType.Annual)
            {
                var remainingDays = employee.AnnualLeaveBalance.GetRemainingDays();

                if (remainingDays < durationDays)
                {
                    _logger.LogWarning(
                        "Yetersiz yıllık izin bakiyesi. EmployeeId: {EmployeeId}, Required: {Required}, Remaining: {Remaining}",
                        request.EmployeeId, durationDays, remainingDays);

                    return Result<LeaveRequestDto>.Failure(
                        $"Yeterli yıllık izin günü yok. İhtiyaç: {durationDays} gün, Kalan: {remainingDays} gün");
                }

                _logger.LogInformation(
                    "İzin bakiyesi yeterli. Remaining: {Remaining} gün, Required: {Required} gün",
                    remainingDays, durationDays);
            }

            // ========== 6. ÇAKIŞAN İZİN KONTROLÜ ==========
            var conflictingLeaves = employee.Leaves
                .Where(l => l.Status == LeaveStatus.Approved &&
                            l.StartDate.Date <= request.EndDate.Date &&
                            l.EndDate.Date >= request.StartDate.Date)
                .ToList();

            if (conflictingLeaves.Any())
            {
                _logger.LogWarning(
                    "Çakışan onaylanmış izin var. EmployeeId: {EmployeeId}, ConflictingCount: {Count}",
                    request.EmployeeId, conflictingLeaves.Count);

                return Result<LeaveRequestDto>.Failure(
                    "Bu tarih aralığında zaten onaylanmış bir izin var.");
            }

            // ========== 7. İZİN OLUŞTURMA ==========
            var leave = Leave.Create(
                request.EmployeeId,
                leaveType,
                request.StartDate,
                request.EndDate,
                request.Reason
            );

            _logger.LogInformation(
                "İzin entity'si oluşturuldu. LeaveId: {LeaveId}, TotalDays: {TotalDays}",
                leave.Id, durationDays);

            // ========== 8. VERITABANINA EKLEME ==========
            await _leaveRepository.AddAsync(leave, cancellationToken);

            // ========== 9. EMPLOYEE ÜZERINDE İZİN TALEP ETME ==========
            try
            {
                employee.RequestLeave(leave);
                _logger.LogInformation("Çalışan üzerinde izin kaydı yapıldı. EmployeeId: {EmployeeId}", request.EmployeeId);
            }
            catch (DomainException ex)
            {
                _logger.LogError(ex, "Domain hatası oluştu. EmployeeId: {EmployeeId}", request.EmployeeId);
                return Result<LeaveRequestDto>.Failure($"İzin talebi işlenirken hata: {ex.Message}");
            }

            // ========== 10. DEĞİŞİKLİKLERİ KAYDETME ==========
            await _employeeRepository.UpdateAsync(employee, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _logger.LogInformation(
                "İzin talebi başarıyla kaydedildi. LeaveId: {LeaveId}, EmployeeId: {EmployeeId}, Status: {Status}",
                leave.Id, request.EmployeeId, leave.Status);

            // ========== 11. DTO HAZIRLAMA VE DÖNÜŞ ==========
            var dto = new LeaveRequestDto
            {
                Id = leave.Id,
                EmployeeId = leave.EmployeeId,
                EmployeeName = $"{employee.Person.FirstName} {employee.Person.LastName}",
                LeaveType = leave.LeaveType.ToString(),
                StartDate = leave.StartDate,
                EndDate = leave.EndDate,
                DurationDays = durationDays,
                Reason = leave.Reason,
                Status = leave.Status.ToString(),
                CreatedAt = leave.CreatedAt
            };

            return Result<LeaveRequestDto>.Success(
                dto,
                $"İzin talebi başarıyla oluşturuldu. Durum: {leave.Status}");
        }
        catch (DomainException ex)
        {
            _logger.LogError(ex, "Domain hatası. EmployeeId: {EmployeeId}", request.EmployeeId);
            return Result<LeaveRequestDto>.Failure($"İş kuralı hatası: {ex.Message}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Beklenmeyen hata. EmployeeId: {EmployeeId}", request.EmployeeId);
            return Result<LeaveRequestDto>.Failure($"İzin talebi işlenirken hata oluştu: {ex.Message}");
        }
    }
}