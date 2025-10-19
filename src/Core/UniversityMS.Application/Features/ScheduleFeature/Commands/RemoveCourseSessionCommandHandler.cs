using MediatR;
using Microsoft.Extensions.Logging;
using UniversityMS.Application.Common.Models;
using UniversityMS.Domain.Entities.ScheduleAggregate;
using UniversityMS.Domain.Interfaces;

namespace UniversityMS.Application.Features.ScheduleFeature.Commands;

public class RemoveCourseSessionCommandHandler : IRequestHandler<RemoveCourseSessionCommand, Result>
{
    private readonly IRepository<Schedule> _scheduleRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<RemoveCourseSessionCommandHandler> _logger;

    public RemoveCourseSessionCommandHandler(
        IRepository<Schedule> scheduleRepository,
        IUnitOfWork unitOfWork,
        ILogger<RemoveCourseSessionCommandHandler> logger)
    {
        _scheduleRepository = scheduleRepository;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Result> Handle(RemoveCourseSessionCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var schedule = await _scheduleRepository.GetByIdAsync(request.ScheduleId, cancellationToken);
            if (schedule == null)
                return Result.Failure("Program bulunamadı.");

            schedule.RemoveCourseSession(request.SessionId);

            await _scheduleRepository.UpdateAsync(schedule, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Course session removed from schedule: {ScheduleId}", request.ScheduleId);

            return Result.Success("Ders programdan çıkarıldı.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error removing course session");
            return Result.Failure("Ders çıkarılırken hata oluştu: " + ex.Message);
        }
    }
}