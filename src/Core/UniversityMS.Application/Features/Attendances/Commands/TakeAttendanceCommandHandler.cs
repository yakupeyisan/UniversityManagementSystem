using MediatR;
using Microsoft.Extensions.Logging;
using UniversityMS.Application.Common.Models;
using UniversityMS.Domain.Entities.EnrollmentAggregate;
using UniversityMS.Domain.Enums;
using UniversityMS.Domain.Interfaces;

namespace UniversityMS.Application.Features.Attendances.Commands;

public class TakeAttendanceCommandHandler : IRequestHandler<TakeAttendanceCommand, Result<int>>
{
    private readonly IRepository<Attendance> _attendanceRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<TakeAttendanceCommandHandler> _logger;

    public TakeAttendanceCommandHandler(
        IRepository<Attendance> attendanceRepository,
        IUnitOfWork unitOfWork,
        ILogger<TakeAttendanceCommandHandler> logger)
    {
        _attendanceRepository = attendanceRepository;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Result<int>> Handle(TakeAttendanceCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var attendances = new List<Attendance>();

            foreach (var attendanceDto in request.Attendances)
            {
                var attendance = Attendance.Create(
                    attendanceDto.CourseRegistrationId,
                    attendanceDto.StudentId,
                    request.CourseId,
                    request.AttendanceDate,
                    request.WeekNumber,
                    attendanceDto.IsPresent,
                    AttendanceMethod.Manual
                );
                attendances.Add(attendance);
            }

            foreach (var attendance in attendances)
            {
                await _attendanceRepository.AddAsync(attendance, cancellationToken);
            }

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Attendance taken for course {CourseId}: {Count} students",
                request.CourseId, attendances.Count);

            return Result.Success(attendances.Count, $"{attendances.Count} öğrenci yoklaması alındı.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error taking attendance");
            return Result.Failure<int>("Yoklama alınırken bir hata oluştu.", ex.Message);
        }
    }
}