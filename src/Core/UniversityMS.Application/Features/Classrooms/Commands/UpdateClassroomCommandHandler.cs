using MediatR;
using Microsoft.Extensions.Logging;
using UniversityMS.Application.Common.Models;
using UniversityMS.Domain.Entities.FacilityAggregate;
using UniversityMS.Domain.Interfaces;

namespace UniversityMS.Application.Features.Classrooms.Commands;

public class UpdateClassroomCommandHandler : IRequestHandler<UpdateClassroomCommand, Result>
{
    private readonly IRepository<Classroom> _classroomRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<UpdateClassroomCommandHandler> _logger;

    public UpdateClassroomCommandHandler(
        IRepository<Classroom> classroomRepository,
        IUnitOfWork unitOfWork,
        ILogger<UpdateClassroomCommandHandler> logger)
    {
        _classroomRepository = classroomRepository;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Result> Handle(UpdateClassroomCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var classroom = await _classroomRepository.GetByIdAsync(request.Id, cancellationToken);
            if (classroom == null)
                return Result.Failure("Derslik bulunamadı.");

            classroom.UpdateBasicInfo(request.Name, request.Capacity);

            if (!string.IsNullOrWhiteSpace(request.Building) && request.Floor.HasValue)
            {
                classroom.SetLocation(request.Building, request.Floor.Value);
            }

            classroom.SetFeatures(
                request.HasProjector,
                request.HasSmartBoard,
                request.HasComputers,
                request.HasAirConditioning,
                request.ComputerCount
            );

            await _classroomRepository.UpdateAsync(classroom, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Classroom updated: {ClassroomId}", request.Id);

            return Result.Success("Derslik bilgileri güncellendi.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating classroom");
            return Result.Failure("Derslik güncellenirken hata oluştu.", ex.Message);
        }
    }
}