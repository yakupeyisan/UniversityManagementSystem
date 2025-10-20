using MediatR;
using Microsoft.Extensions.Logging;
using UniversityMS.Application.Common.Models;
using UniversityMS.Domain.Entities.ScheduleAggregate;
using UniversityMS.Domain.Enums;
using UniversityMS.Domain.Interfaces;

namespace UniversityMS.Application.Features.ScheduleFeature.Commands;

public class UpdateScheduleCommandHandler : IRequestHandler<UpdateScheduleCommand, Result<Guid>>
{
    private readonly IRepository<Schedule> _repository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<UpdateScheduleCommandHandler> _logger;

    public UpdateScheduleCommandHandler(
        IRepository<Schedule> repository,
        IUnitOfWork unitOfWork,
        ILogger<UpdateScheduleCommandHandler> logger)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Result<Guid>> Handle(
        UpdateScheduleCommand request,
        CancellationToken cancellationToken)
    {
        try
        {
            // Mevcut programı getir
            var schedule = await _repository.GetByIdAsync(request.Id, cancellationToken);
            if (schedule == null)
                return Result<Guid>.Failure("Program bulunamadı.");

            // Aktif programa statü değiştirmemeye izin verme
            if (schedule.Status == ScheduleStatus.Active && request.Status != ScheduleStatus.Active)
                return Result<Guid>.Failure("Aktif programa statü değiştirilemez.");

            // Alanları güncelle
            schedule.Update(
                request.Name,
                request.Description,
                request.StartDate,
                request.EndDate,
                request.Status);


            await _repository.UpdateAsync(schedule, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _logger.LogInformation(
                "Schedule updated successfully: {ScheduleId} | Name: {Name} | Status: {Status}",
                request.Id, request.Name, request.Status);

            return Result<Guid>.Success(
                schedule.Id,
                "Ders programı başarıyla güncellendi.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating schedule: {ScheduleId}", request.Id);
            return Result<Guid>.Failure($"Program güncellenirken hata oluştu: {ex.Message}");
        }
    }
}