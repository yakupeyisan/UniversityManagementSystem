using MediatR;
using Microsoft.Extensions.Logging;
using UniversityMS.Application.Common.Models;
using UniversityMS.Domain.Entities.FacilityAggregate;
using UniversityMS.Domain.Interfaces;

namespace UniversityMS.Application.Features.Classrooms.Commands;

public class ToggleClassroomActiveCommandHandler : IRequestHandler<ToggleClassroomActiveCommand, Result>
{
    private readonly IRepository<Classroom> _classroomRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<ToggleClassroomActiveCommandHandler> _logger;

    public ToggleClassroomActiveCommandHandler(
        IRepository<Classroom> classroomRepository,
        IUnitOfWork unitOfWork,
        ILogger<ToggleClassroomActiveCommandHandler> logger)
    {
        _classroomRepository = classroomRepository;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Result> Handle(ToggleClassroomActiveCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var classroom = await _classroomRepository.GetByIdAsync(request.Id, cancellationToken);
            if (classroom == null)
                return Result.Failure("Derslik bulunamadı.");

            if (classroom.IsActive)
                classroom.Deactivate();
            else
                classroom.Activate();

            await _classroomRepository.UpdateAsync(classroom, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            var status = classroom.IsActive ? "aktif" : "pasif";
            _logger.LogInformation("Classroom toggled: {ClassroomId} - {Status}", request.Id, status);

            return Result.Success($"Derslik {status} hale getirildi.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error toggling classroom");
            return Result.Failure("Derslik durumu değiştirilirken hata oluştu.", ex.Message);
        }
    }
}