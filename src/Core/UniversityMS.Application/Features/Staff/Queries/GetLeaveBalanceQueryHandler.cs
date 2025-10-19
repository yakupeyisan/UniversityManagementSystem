using MediatR;
using UniversityMS.Application.Common.Models;
using UniversityMS.Application.Features.Staff.DTOs;
using UniversityMS.Domain.Entities.HRAggregate;
using UniversityMS.Domain.Interfaces;

namespace UniversityMS.Application.Features.Staff.Queries;

public class GetLeaveBalanceQueryHandler : IRequestHandler<GetLeaveBalanceQuery, Result<LeaveBalanceDto>>
{
    private readonly IRepository<Employee> _employeeRepository;

    public GetLeaveBalanceQueryHandler(IRepository<Employee> employeeRepository)
    {
        _employeeRepository = employeeRepository;
    }

    public async Task<Result<LeaveBalanceDto>> Handle(
        GetLeaveBalanceQuery request,
        CancellationToken cancellationToken)
    {
        var employee = await _employeeRepository.GetByIdAsync(request.EmployeeId, cancellationToken);
        if (employee == null)
            return Result<LeaveBalanceDto>.Failure("Çalışan bulunamadı");

        return Result<LeaveBalanceDto>.Success(new LeaveBalanceDto
        {
            AnnualLeaveDays = employee.AnnualLeaveBalance.AnnualLeaveDays,
            SickLeaveDays = employee.AnnualLeaveBalance.SickLeaveDays,
            RemainingAnnualDays = employee.AnnualLeaveBalance.RemainingAnnualDays,
            RemainingSickDays = employee.AnnualLeaveBalance.RemainingSickDays
        });
    }
}