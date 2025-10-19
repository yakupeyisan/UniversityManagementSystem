using MediatR;
using Microsoft.Extensions.Logging;
using UniversityMS.Application.Common.Models;
using UniversityMS.Application.Features.Staff.DTOs;
using UniversityMS.Domain.Entities.HRAggregate;
using UniversityMS.Domain.Enums;
using UniversityMS.Domain.Exceptions;
using UniversityMS.Domain.Interfaces;

namespace UniversityMS.Application.Features.Staff.Commands;

public class ApplyLeaveCommandHandler : IRequestHandler<ApplyLeaveCommand, Result<LeaveRequestDto>>
{
    private readonly IRepository<Employee> _employeeRepository;
    private readonly IRepository<Leave> _leaveRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<ApplyLeaveCommandHandler> _logger;

    public ApplyLeaveCommandHandler(
        IRepository<Employee> employeeRepository,
        IRepository<Leave> leaveRepository,
        IUnitOfWork unitOfWork,
        ILogger<ApplyLeaveCommandHandler> logger)
    {
        _employeeRepository = employeeRepository;
        _leaveRepository = leaveRepository;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Result<LeaveRequestDto>> Handle(
        ApplyLeaveCommand request,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation(
            "Processing leave request for employee {EmployeeId}, LeaveType: {LeaveType}",
            request.EmployeeId,
            request.LeaveTypeId);

        try
        {
            // Employee bul
            var employee = await _employeeRepository.GetByIdAsync(request.EmployeeId, cancellationToken);
            if (employee == null)
            {
                _logger.LogWarning("Employee not found: {EmployeeId}", request.EmployeeId);
                return Result<LeaveRequestDto>.Failure("Çalışan bulunamadı");
            }

            // Tarih hesapla
            var durationDays = (request.EndDate.Date - request.StartDate.Date).Days + 1;
            var leaveType = (LeaveType)request.LeaveTypeId;

            // ✅ DÜZELTILMIŞ: LeaveBalance kontrolü basit
            if (!employee.AnnualLeaveBalance.CanTakeLeave(durationDays))
            {
                var remaining = employee.AnnualLeaveBalance.GetRemainingDays();
                _logger.LogWarning(
                    "Insufficient leave balance for {EmployeeId}. Required: {Required}, Remaining: {Remaining}",
                    request.EmployeeId,
                    durationDays,
                    remaining);

                return Result<LeaveRequestDto>.Failure(
                    $"Yeterli izin günü yok. Kalan: {remaining} gün");
            }

            // Leave oluştur
            var leave = Leave.Create(
                request.EmployeeId,
                leaveType,
                request.StartDate,
                request.EndDate,
                request.Reason
            );

            await _leaveRepository.AddAsync(leave, cancellationToken);

            // Employee üzerinden izin talep et
            employee.RequestLeave(leave);

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _logger.LogInformation(
                "Leave request created successfully. EmployeeId: {EmployeeId}, LeaveId: {LeaveId}",
                request.EmployeeId,
                leave.Id);

            return Result<LeaveRequestDto>.Success(new LeaveRequestDto
            {
                Id = leave.Id,
                EmployeeId = leave.EmployeeId,
                LeaveType = leave.LeaveType.ToString(),
                StartDate = leave.StartDate,
                EndDate = leave.EndDate,
                DurationDays = durationDays,
                Status = leave.Status.ToString()
            });
        }
        catch (DomainException ex)
        {
            _logger.LogError(ex, "Domain error while processing leave request");
            return Result<LeaveRequestDto>.Failure(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error while processing leave request");
            return Result<LeaveRequestDto>.Failure("İzin talebi işlenirken hata oluştu");
        }
    }
}