using MediatR;
using Microsoft.Extensions.Logging;
using UniversityMS.Application.Common.Models;
using UniversityMS.Domain.Entities.ScheduleAggregate;
using UniversityMS.Domain.Interfaces;

namespace UniversityMS.Application.Features.ScheduleFeature.Commands;

public class AddCourseSessionCommandHandler : IRequestHandler<AddCourseSessionCommand, Result<Guid>>
{
    private readonly IRepository<Schedule> _scheduleRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<AddCourseSessionCommandHandler> _logger;

    public AddCourseSessionCommandHandler(
        IRepository<Schedule> scheduleRepository,
        IUnitOfWork unitOfWork,
        ILogger<AddCourseSessionCommandHandler> logger)
    {
        _scheduleRepository = scheduleRepository;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Result<Guid>> Handle(AddCourseSessionCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var schedule = await _scheduleRepository.GetByIdAsync(request.ScheduleId, cancellationToken);
            if (schedule == null)
                return Result<Guid>.Failure("Program bulunamadı.");

            // Parse times
            if (!TimeSpan.TryParse(request.StartTime, out var startTime))
                return Result<Guid>.Failure("Geçersiz başlangıç saati.");

            if (!TimeSpan.TryParse(request.EndTime, out var endTime))
                return Result<Guid>.Failure("Geçersiz bitiş saati.");

            schedule.AddCourseSession(
                request.CourseId,
                request.InstructorId,
                request.ClassroomId,
                request.DayOfWeek,
                startTime,
                endTime,
                request.SessionType
            );

            await _scheduleRepository.UpdateAsync(schedule, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            // Get the newly added session ID (last one)
            var newSession = schedule.CourseSessions.LastOrDefault();
            var sessionId = newSession?.Id ?? Guid.Empty;

            _logger.LogInformation("Course session added to schedule: {ScheduleId}", request.ScheduleId);

            return Result<Guid>.Success(sessionId, "Ders programı güncellendi.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error adding course session");
            return Result<Guid>.Failure("Ders eklenirken hata oluştu: " + ex.Message);
        }
    }
}