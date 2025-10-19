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
            // CourseRegistration kontrol et
            var courseRegistration = await _courseRegistrationRepository.GetByIdAsync(
                request.CourseRegistrationId, cancellationToken);

            if (courseRegistration == null)
            {
                _logger.LogWarning("CourseRegistration not found: {CourseRegistrationId}",
                    request.CourseRegistrationId);
                return Result.Failure("Ders kaydı bulunamadı.");
            }

            // Attendance oluştur
            var attendance = Attendance.Create(
                request.CourseRegistrationId,
                request.WeekNumber,
                request.IsPresent
            );

            await _attendanceRepository.AddAsync(attendance, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Attendance recorded for CourseRegistration: {CourseRegistrationId}, Week: {Week}",
                request.CourseRegistrationId, request.WeekNumber);

            return Result.Success("Devam kaydı başarıyla alındı.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error taking attendance");
            return Result.Failure("Devam kaydı alınırken bir hata oluştu.", ex.Message);
        }
    }
}