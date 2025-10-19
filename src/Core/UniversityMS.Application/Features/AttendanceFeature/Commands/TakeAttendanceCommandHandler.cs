using MediatR;
using Microsoft.Extensions.Logging;
using UniversityMS.Application.Common.Models;
using UniversityMS.Domain.Entities.EnrollmentAggregate;
using UniversityMS.Domain.Enums;
using UniversityMS.Domain.Interfaces;

namespace UniversityMS.Application.Features.AttendanceFeature.Commands;

public class TakeAttendanceCommandHandler : IRequestHandler<TakeAttendanceCommand, Result>
{
    private readonly IRepository<Attendance> _attendanceRepository;
    private readonly IRepository<CourseRegistration> _courseRegistrationRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<TakeAttendanceCommandHandler> _logger;

    public TakeAttendanceCommandHandler(
        IRepository<Attendance> attendanceRepository,
        IRepository<CourseRegistration> courseRegistrationRepository,
        IUnitOfWork unitOfWork,
        ILogger<TakeAttendanceCommandHandler> logger)
    {
        _attendanceRepository = attendanceRepository;
        _courseRegistrationRepository = courseRegistrationRepository;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Result> Handle(TakeAttendanceCommand request, CancellationToken cancellationToken)
    {
        try
        {
            // Her bir student için Attendance kaydı oluştur
            foreach (var attendanceDto in request.Attendances)
            {
                // CourseRegistration'ı bul
                var courseRegistrations = await _courseRegistrationRepository.FindAsync(
                    cr => cr.Enrollment.StudentId == attendanceDto.StudentId && cr.CourseId == request.CourseId,
                    cancellationToken);

                if (!courseRegistrations.Any())
                {
                    _logger.LogWarning("CourseRegistration not found for StudentId: {StudentId}",
                        attendanceDto.StudentId);
                    continue;
                }

                var courseRegistration = courseRegistrations.First();

                // Attendance oluştur
                var attendance = Attendance.Create(
                    courseRegistration.Id,
                    attendanceDto.StudentId,
                    request.CourseId,
                    request.AttendanceDate,
                    request.WeekNumber,
                    attendanceDto.IsPresent,
                    AttendanceMethod.Manual
                );

                // Notları ekle
                if (!string.IsNullOrEmpty(attendanceDto.Notes))
                    attendance.AddNotes(attendanceDto.Notes);

                await _attendanceRepository.AddAsync(attendance, cancellationToken);
            }

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Attendance recorded for Course: {CourseId}, Week: {Week}, Date: {Date}",
                request.CourseId, request.WeekNumber, request.AttendanceDate);

            return Result.Success("Devam kaydı başarıyla alındı.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error taking attendance for CourseId: {CourseId}", request.CourseId);
            return Result.Failure("Devam kaydı alınırken bir hata oluştu.", ex.Message);
        }
    }
}