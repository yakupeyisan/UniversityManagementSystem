using MediatR;
using Microsoft.Extensions.Logging;
using UniversityMS.Application.Common.Interfaces;
using UniversityMS.Application.Common.Models;
using UniversityMS.Domain.Entities.ScheduleAggregate;
using UniversityMS.Domain.Interfaces;

namespace UniversityMS.Application.Features.ScheduleFeature.Commands;

public class PublishScheduleCommandHandler : IRequestHandler<PublishScheduleCommand, Result>
{
    private readonly IRepository<Schedule> _scheduleRepository;
    private readonly ICurrentUserService _currentUserService;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<PublishScheduleCommandHandler> _logger;

    public PublishScheduleCommandHandler(
        IRepository<Schedule> scheduleRepository,
        ICurrentUserService currentUserService,
        IUnitOfWork unitOfWork,
        ILogger<PublishScheduleCommandHandler> logger)
    {
        _scheduleRepository = scheduleRepository;
        _currentUserService = currentUserService;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Result> Handle(PublishScheduleCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var schedule = await _scheduleRepository.GetByIdAsync(request.ScheduleId, cancellationToken);
            if (schedule == null)
                return Result.Failure("Program bulunamadı.");

            var userId = _currentUserService.UserId;
            if (userId == Guid.Empty)
                return Result.Failure("Kullanıcı bilgisi alınamadı.");
            schedule.Publish(userId);

            await _scheduleRepository.UpdateAsync(schedule, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Schedule published: {ScheduleId}", request.ScheduleId);

            return Result.Success("Program yayınlandı.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error publishing schedule");
            return Result.Failure("Program yayınlanırken hata oluştu: " + ex.Message);
        }
    }
}