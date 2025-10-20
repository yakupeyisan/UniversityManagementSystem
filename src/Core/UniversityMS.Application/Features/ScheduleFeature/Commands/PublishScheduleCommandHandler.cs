using MediatR;
using Microsoft.Extensions.Logging;
using UniversityMS.Application.Common.Interfaces;
using UniversityMS.Application.Common.Models;
using UniversityMS.Domain.Entities.ScheduleAggregate;
using UniversityMS.Domain.Interfaces;

namespace UniversityMS.Application.Features.ScheduleFeature.Commands;

public class PublishScheduleCommandHandler
    : IRequestHandler<PublishScheduleCommand, Result<Guid>>
{
    private readonly IRepository<Schedule> _repository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<PublishScheduleCommandHandler> _logger;
    private readonly ICurrentUserService _currentUserService;

    public PublishScheduleCommandHandler(
        IRepository<Schedule> repository,
        IUnitOfWork unitOfWork,
        ILogger<PublishScheduleCommandHandler> logger,
        ICurrentUserService currentUserService)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
        _logger = logger;
        _currentUserService = currentUserService;
    }

    public async Task<Result<Guid>> Handle(
        PublishScheduleCommand request,
        CancellationToken cancellationToken)
    {
        try
        {
            var schedule = await _repository.GetByIdAsync(request.ScheduleId, cancellationToken);
            if (schedule == null)
                return Result<Guid>.Failure("Program bulunamadı.");

            var userId = _currentUserService.UserId;
            if (userId == Guid.Empty)
                return Result<Guid>.Failure("Kullanıcı bilgisi alınamadı.");

            schedule.Publish(userId);

            await _repository.UpdateAsync(schedule, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _logger.LogInformation(
                "Schedule published: {ScheduleId} | Academic Year: {AcademicYear}",
                request.ScheduleId, schedule.AcademicYear);

            return Result<Guid>.Success(
                schedule.Id,
                "Ders programı yayınlandı.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error publishing schedule");
            return Result<Guid>.Failure($"Hata: {ex.Message}");
        }
    }
}