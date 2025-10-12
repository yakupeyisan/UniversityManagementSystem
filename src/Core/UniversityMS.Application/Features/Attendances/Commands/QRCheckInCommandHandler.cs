using MediatR;
using Microsoft.Extensions.Logging;
using UniversityMS.Application.Common.Models;
using UniversityMS.Domain.Entities.EnrollmentAggregate;
using UniversityMS.Domain.Enums;
using UniversityMS.Domain.Interfaces;

namespace UniversityMS.Application.Features.Attendances.Commands;

public class QRCheckInCommandHandler : IRequestHandler<QRCheckInCommand, Result>
{
    private readonly IRepository<Attendance> _attendanceRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<QRCheckInCommandHandler> _logger;

    public QRCheckInCommandHandler(
        IRepository<Attendance> attendanceRepository,
        IUnitOfWork unitOfWork,
        ILogger<QRCheckInCommandHandler> logger)
    {
        _attendanceRepository = attendanceRepository;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Result> Handle(QRCheckInCommand request, CancellationToken cancellationToken)
    {
        try
        {
            // TODO: Validate QR code and check expiration
            // QR kod formatı: COURSE_ID|DATE|HASH şeklinde olabilir
            // Hash doğrulaması yapılmalı

            var attendance = Attendance.Create(
                request.CourseRegistrationId,
                request.StudentId,
                request.CourseId,
                DateTime.UtcNow,
                request.WeekNumber,
                true, // QR ile giriş yapıldığı için present
                AttendanceMethod.QRCode
            );

            await _attendanceRepository.AddAsync(attendance, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("QR check-in successful for student {StudentId} in course {CourseId}",
                request.StudentId, request.CourseId);

            return Result.Success("Yoklama başarıyla kaydedildi.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during QR check-in");
            return Result.Failure("QR ile yoklama alınırken bir hata oluştu.", ex.Message);
        }
    }
}