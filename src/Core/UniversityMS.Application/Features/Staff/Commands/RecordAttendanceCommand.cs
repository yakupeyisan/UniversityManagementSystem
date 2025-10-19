using MediatR;
using UniversityMS.Application.Common.Models;
using UniversityMS.Application.Features.Attendances.DTOs;

namespace UniversityMS.Application.Features.Staff.Commands;

public class RecordAttendanceCommand : IRequest<Result<AttendanceDto>>
{
    public Guid EmployeeId { get; set; }
    public DateTime CheckInTime { get; set; }
    public string Location { get; set; } = string.Empty;
}