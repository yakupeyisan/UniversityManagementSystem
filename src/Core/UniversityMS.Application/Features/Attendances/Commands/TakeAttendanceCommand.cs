using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;
using UniversityMS.Application.Common.Models;
using UniversityMS.Application.Features.Attendances.DTOs;
using UniversityMS.Domain.Entities.EnrollmentAggregate;
using UniversityMS.Domain.Interfaces;

namespace UniversityMS.Application.Features.Attendances.Commands;

public record TakeAttendanceCommand(
    Guid CourseId,
    Guid InstructorId,
    DateTime AttendanceDate,
    List<AttendanceDto> Attendances
) : IRequest<Result<int>>;

public class TakeAttendanceCommandValidator : AbstractValidator<TakeAttendanceCommand>
{
    public TakeAttendanceCommandValidator()
    {
        RuleFor(x => x.CourseId)
            .NotEmpty().WithMessage("Ders ID gereklidir.");

        RuleFor(x => x.InstructorId)
            .NotEmpty().WithMessage("Öğretim görevlisi ID gereklidir.");

        RuleFor(x => x.AttendanceDate)
            .NotEmpty().WithMessage("Yoklama tarihi gereklidir.")
            .LessThanOrEqualTo(DateTime.UtcNow).WithMessage("Yoklama tarihi gelecekte olamaz.");

        RuleFor(x => x.Attendances)
            .NotEmpty().WithMessage("En az bir öğrenci yoklaması girilmelidir.");

        RuleForEach(x => x.Attendances).ChildRules(attendance =>
        {
            attendance.RuleFor(x => x.StudentId)
                .NotEmpty().WithMessage("Öğrenci ID gereklidir.");
        });
    }
}

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
                    Guid.NewGuid(), // CourseRegistrationId - should be fetched properly
                    attendanceDto.StudentId,
                    request.CourseId,
                    request.InstructorId,
                    request.AttendanceDate,
                    attendanceDto.IsPresent
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

public record QRCheckInCommand(
    Guid StudentId,
    Guid CourseId,
    string QRCode
) : IRequest<Result>;

public class QRCheckInCommandValidator : AbstractValidator<QRCheckInCommand>
{
    public QRCheckInCommandValidator()
    {
        RuleFor(x => x.StudentId)
            .NotEmpty().WithMessage("Öğrenci ID gereklidir.");

        RuleFor(x => x.CourseId)
            .NotEmpty().WithMessage("Ders ID gereklidir.");

        RuleFor(x => x.QRCode)
            .NotEmpty().WithMessage("QR kodu gereklidir.");
    }
}

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
            // TODO: Get CourseRegistrationId properly

            var attendance = Attendance.Create(
                Guid.NewGuid(), // CourseRegistrationId - should be fetched properly
                request.StudentId,
                request.CourseId,
                null, // InstructorId - can be null for QR check-in
                DateTime.UtcNow,
                true
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