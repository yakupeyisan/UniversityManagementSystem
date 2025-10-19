using MediatR;
using UniversityMS.Application.Common.Models;
using UniversityMS.Application.Features.Attendances.DTOs;
using UniversityMS.Domain.Entities.EnrollmentAggregate;
using UniversityMS.Domain.Entities.HRAggregate;
using UniversityMS.Domain.Interfaces;

namespace UniversityMS.Application.Features.Staff.Commands;

public class RecordAttendanceCommandHandler : IRequestHandler<RecordAttendanceCommand, Result<AttendanceDto>>
{
    private readonly IRepository<Shift> _shiftRepository;
    private readonly IRepository<Attendance> _attendanceRepository;
    private readonly IUnitOfWork _unitOfWork;

    public RecordAttendanceCommandHandler(
        IRepository<Shift> shiftRepository,
        IRepository<Attendance> attendanceRepository,
        IUnitOfWork unitOfWork)
    {
        _shiftRepository = shiftRepository;
        _attendanceRepository = attendanceRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<AttendanceDto>> Handle(
        RecordAttendanceCommand request,
        CancellationToken cancellationToken)
    {
        var today = request.CheckInTime.Date;
        var attendance = Attendance.Create(
            request.EmployeeId,
            request.CheckInTime,
            request.Location
        );

        await _attendanceRepository.AddAsync(attendance, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result<AttendanceDto>.Success(new AttendanceDto
        {
            Id = attendance.Id,
            EmployeeId = attendance.EmployeeId,
            CheckInTime = attendance.CheckInTime,
            Location = attendance.Location,
            Status = attendance.Status.ToString()
        });
    }
}