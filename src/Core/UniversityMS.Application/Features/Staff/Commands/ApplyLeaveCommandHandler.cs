using MediatR;
using UniversityMS.Application.Common.Models;
using UniversityMS.Application.Features.Staff.DTOs;
using UniversityMS.Domain.Entities.HRAggregate;
using UniversityMS.Domain.Enums;
using UniversityMS.Domain.Interfaces;

namespace UniversityMS.Application.Features.Staff.Commands;

public class ApplyLeaveCommandHandler : IRequestHandler<ApplyLeaveCommand, Result<LeaveRequestDto>>
{
    private readonly IRepository<Employee> _employeeRepository;
    private readonly IRepository<Leave> _leaveRepository;
    private readonly IUnitOfWork _unitOfWork;

    public ApplyLeaveCommandHandler(
        IRepository<Employee> employeeRepository,
        IRepository<Leave> leaveRepository,
        IUnitOfWork unitOfWork)
    {
        _employeeRepository = employeeRepository;
        _leaveRepository = leaveRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<LeaveRequestDto>> Handle(
        ApplyLeaveCommand request,
        CancellationToken cancellationToken)
    {
        var employee = await _employeeRepository.GetByIdAsync(request.EmployeeId, cancellationToken);
        if (employee == null)
            return Result<LeaveRequestDto>.Failure("Çalışan bulunamadı");

        // Duration hesapla
        var durationDays = (request.EndDate.Date - request.StartDate.Date).Days + 1;

        // Leave hesapla - AnnualLeaveBalance'den direkt okuma
        var annualLeaveDays = employee.AnnualLeaveBalance?.AnnualLeaveDays ?? 0;
        var sickLeaveDays = employee.AnnualLeaveBalance?.SickLeaveDays ?? 0;

        var availableDays = request.LeaveTypeId == 1 ? annualLeaveDays : sickLeaveDays;

        if (durationDays > availableDays)
            return Result<LeaveRequestDto>.Failure($"Yeterli izin günü yok. Kalan: {availableDays}");

        var leave = Leave.Create(
            request.EmployeeId,
            (LeaveType)request.LeaveTypeId,
            request.StartDate,
            request.EndDate,
            request.Reason
        );

        await _leaveRepository.AddAsync(leave, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result<LeaveRequestDto>.Success(new LeaveRequestDto
        {
            Id = leave.Id,
            EmployeeId = leave.EmployeeId,
            LeaveType = leave.LeaveType.ToString(),
            StartDate = leave.StartDate,
            EndDate = leave.EndDate,
            // Duration Days - Leave entity'den doğrudan oku
            DurationDays = (leave.EndDate - leave.StartDate).Days + 1,
            Status = leave.Status.ToString()
        });
    }
}
