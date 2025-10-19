using MediatR;
using UniversityMS.Application.Common.Models;
using UniversityMS.Application.Features.StaffFeature.DTOs;

namespace UniversityMS.Application.Features.StaffFeature.Queries;

public class GetLeaveBalanceQuery : IRequest<Result<LeaveBalanceDto>>
{
    public Guid EmployeeId { get; set; }
}