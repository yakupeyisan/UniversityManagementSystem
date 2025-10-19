using MediatR;
using UniversityMS.Application.Common.Models;
using UniversityMS.Application.Features.Staff.DTOs;

namespace UniversityMS.Application.Features.Staff.Commands;

public class ApplyLeaveCommand : IRequest<Result<LeaveRequestDto>>
{
    public Guid EmployeeId { get; set; }
    public int LeaveTypeId { get; set; } // 1=Annual, 2=Sick, 3=Unpaid
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public string Reason { get; set; } = string.Empty;
}