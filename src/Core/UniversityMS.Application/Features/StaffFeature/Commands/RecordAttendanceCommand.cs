using MediatR;
using UniversityMS.Application.Common.Models;
using UniversityMS.Application.Features.StaffFeature.DTOs;

namespace UniversityMS.Application.Features.StaffFeature.Commands;

public class RecordAttendanceCommand : IRequest<Result<AttendanceDto>>
{
    public Guid EmployeeId { get; set; }
    public DateTime CheckInTime { get; set; }
    public string Location { get; set; } = string.Empty;
}