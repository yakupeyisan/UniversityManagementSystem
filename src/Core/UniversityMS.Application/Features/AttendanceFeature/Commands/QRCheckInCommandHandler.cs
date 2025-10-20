using MediatR;
using Microsoft.Extensions.Logging;
using UniversityMS.Application.Common.Models;
using UniversityMS.Domain.Entities.EnrollmentAggregate;
using UniversityMS.Domain.Enums;
using UniversityMS.Domain.Interfaces;

namespace UniversityMS.Application.Features.AttendanceFeature.Commands;

public class QRCheckInCommandHandler : IRequestHandler<QRCheckInCommand, Result>
{
    private readonly IRepository<Attendance> _attendanceRepository;
    private readonly IRepository<CourseRegistration> _courseRegistrationRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<QRCheckInCommandHandler> _logger;

    public QRCheckInCommandHandler(
        IRepository<Attendance> attendanceRepository,
        IRepository<CourseRegistration> courseRegistrationRepository,
        IUnitOfWork unitOfWork,
        ILogger<QRCheckInCommandHandler> logger)
    {
        _attendanceRepository = attendanceRepository;
        _courseRegistrationRepository = courseRegistrationRepository;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Result> Handle(QRCheckInCommand request, CancellationToken cancellationToken)
    {
        try
        {
            // QR Kodu doğrula
            if (string.IsNullOrWhiteSpace(request.QRCode))
                return Result.Failure("QR kodu boş olamaz.");

            var courseRegistration = await _courseRegistrationRepository.GetByIdAsync(
                request.CourseRegistrationId, cancellationToken);

            if (courseRegistration == null)
                return Result.Failure("Ders kaydı bulunamadı.");

            var attendance = Attendance.Create(
                request.CourseRegistrationId,
                request.StudentId,              // ✅ Fixed: attendanceDto → request.StudentId
                request.CourseId,
                DateTime.UtcNow,
                request.WeekNumber,
                true,                           // QR ile check-in = present
                AttendanceMethod.QRCode         // ✅ Fixed: AttendanceMethod.QR → QRCode
            );

            await _attendanceRepository.AddAsync(attendance, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("QR Check-in successful for Student: {StudentId}", request.StudentId);

            return Result.Success("QR check-in başarılı.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in QR check-in");
            return Result.Failure("QR check-in sırasında bir hata oluştu.", ex.Message);
        }
    }
}