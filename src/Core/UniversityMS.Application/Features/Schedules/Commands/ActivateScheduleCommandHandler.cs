using MediatR;
using Microsoft.Extensions.Logging;
using UniversityMS.Application.Common.Models;
using UniversityMS.Domain.Entities.ScheduleAggregate;
using UniversityMS.Domain.Interfaces;

namespace UniversityMS.Application.Features.Schedules.Commands;

public class ActivateScheduleCommandHandler : IRequestHandler<ActivateScheduleCommand, Result>
{
    private readonly IRepository<Schedule> _scheduleRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<ActivateScheduleCommandHandler> _logger;

    public ActivateScheduleCommandHandler(
        IRepository<Schedule> scheduleRepository,
        IUnitOfWork unitOfWork,
        ILogger<ActivateScheduleCommandHandler> logger)
    {
        _scheduleRepository = scheduleRepository;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Result> Handle(ActivateScheduleCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var schedule = await _scheduleRepository.GetByIdAsync(request.ScheduleId, cancellationToken);
            if (schedule == null)
                return Result.Failure("Program bulunamadı.");

            schedule.Activate();

            await _scheduleRepository.UpdateAsync(schedule, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Schedule activated: {ScheduleId}", request.ScheduleId);

            return Result.Success("Program aktif hale getirildi.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error activating schedule");
            return Result.Failure("Program aktifleştirilirken hata oluştu: " + ex.Message);
        }
    }
}