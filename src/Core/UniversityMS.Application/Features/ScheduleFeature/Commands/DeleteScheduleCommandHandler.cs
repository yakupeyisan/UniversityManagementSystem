using MediatR;
using Microsoft.Extensions.Logging;
using UniversityMS.Application.Common.Models;
using UniversityMS.Domain.Entities.ScheduleAggregate;
using UniversityMS.Domain.Interfaces;

namespace UniversityMS.Application.Features.ScheduleFeature.Commands;

public class DeleteScheduleCommandHandler
    : IRequestHandler<DeleteScheduleCommand, Result<Guid>>
{
    private readonly IRepository<Schedule> _repository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<DeleteScheduleCommandHandler> _logger;

    public DeleteScheduleCommandHandler(
        IRepository<Schedule> repository,
        IUnitOfWork unitOfWork,
        ILogger<DeleteScheduleCommandHandler> logger)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Result<Guid>> Handle(
        DeleteScheduleCommand request,
        CancellationToken cancellationToken)
    {
        try
        {
            var schedule = await _repository.GetByIdAsync(request.Id, cancellationToken);
            if (schedule == null)
                return Result<Guid>.Failure("Program bulunamadı.");

            // Soft delete
            schedule.IsDeleted = true;
            schedule.DeletedAt = DateTime.UtcNow;

            await _repository.UpdateAsync(schedule, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Schedule deleted: {ScheduleId}", request.Id);

            return Result<Guid>.Success(
                schedule.Id,
                "Ders programı silindi.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting schedule");
            return Result<Guid>.Failure($"Hata: {ex.Message}");
        }
    }
}