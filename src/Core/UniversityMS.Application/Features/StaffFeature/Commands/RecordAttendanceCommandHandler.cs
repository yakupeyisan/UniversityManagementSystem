using MediatR;
using UniversityMS.Application.Common.Models;
using UniversityMS.Application.Features.StaffFeature.DTOs;
using UniversityMS.Domain.Entities.HRAggregate;
using UniversityMS.Domain.Interfaces;

namespace UniversityMS.Application.Features.StaffFeature.Commands;

public class RecordAttendanceCommandHandler : IRequestHandler<RecordAttendanceCommand, Result<AttendanceDto>>
{
    private readonly IRepository<Employee> _employeeRepository;
    private readonly IUnitOfWork _unitOfWork;

    public RecordAttendanceCommandHandler(
        IRepository<Employee> employeeRepository,
        IUnitOfWork unitOfWork)
    {
        _employeeRepository = employeeRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<AttendanceDto>> Handle(
        RecordAttendanceCommand request,
        CancellationToken cancellationToken)
    {
        // Çalışan var mı kontrol et
        var employee = await _employeeRepository.GetByIdAsync(request.EmployeeId, cancellationToken);
        if (employee == null)
            return Result<AttendanceDto>.Failure("Çalışan bulunamadı");

        // EmployeeAttendance kaydı oluştur
        var attendanceDto = new AttendanceDto
        {
            Id = Guid.NewGuid(),
            EmployeeId = request.EmployeeId,
            CheckInTime = request.CheckInTime,
            CheckOutTime = null,
            Location = request.Location,
            Status = "Checked In",
            WorkingHours = 0m
        };

        // TODO: Gerçek Employee Attendance entity'si oluşturulmalı ve kaydedilmeli
        // Şu anda sadece DTO döndürüyoruz

        return Result<AttendanceDto>.Success(attendanceDto);
    }
}