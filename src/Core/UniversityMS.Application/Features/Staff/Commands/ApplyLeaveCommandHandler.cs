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

        var leaveType = (LeaveType)request.LeaveTypeId;
        var balance = employee.AnnualLeaveBalance;

        var durationDays = (request.EndDate.Date - request.StartDate.Date).Days + 1;
        var availableDays = leaveType == LeaveType.Annual ? balance.AnnualLeaveDays :
            leaveType == LeaveType.Sick ? balance.SickLeaveDays : 0;

        if (durationDays > availableDays)
            return Result<LeaveRequestDto>.Failure("Yeterli izin günü yok");

        var leave = Leave.Create(
            request.EmployeeId,
            leaveType,
            request.StartDate,
            request.EndDate,
            request.Reason
        );

        await _leaveRepository.AddAsync(leave, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result<LeaveRequestDto>.Success(new LeaveRequestDto
        {
            Id = leave.Id,
            LeaveType = leave.LeaveType.ToString(),
            StartDate = leave.StartDate,
            EndDate = leave.EndDate,
            DurationDays = leave.DurationDays,
            Status = leave.Status.ToString()
        });
    }
}