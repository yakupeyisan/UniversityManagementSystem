using MediatR;
using Microsoft.Extensions.Logging;
using UniversityMS.Application.Common.Models;
using UniversityMS.Domain.Entities.FacilityAggregate;
using UniversityMS.Domain.Interfaces;

namespace UniversityMS.Application.Features.Classrooms.Commands;

public class CreateClassroomCommandHandler : IRequestHandler<CreateClassroomCommand, Result<Guid>>
{
    private readonly IRepository<Classroom> _classroomRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<CreateClassroomCommandHandler> _logger;

    public CreateClassroomCommandHandler(
        IRepository<Classroom> classroomRepository,
        IUnitOfWork unitOfWork,
        ILogger<CreateClassroomCommandHandler> logger)
    {
        _classroomRepository = classroomRepository;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Result<Guid>> Handle(CreateClassroomCommand request, CancellationToken cancellationToken)
    {
        try
        {
            // Check if classroom code already exists
            var existingClassroom = await _classroomRepository.FirstOrDefaultAsync(
                c => c.Code == request.Code && !c.IsDeleted,
                cancellationToken
            );

            if (existingClassroom != null)
                return Result<Guid>.Failure("Bu kodda bir derslik zaten mevcut.");

            var classroom = Classroom.Create(request.Code, request.Name, request.Capacity, request.Type);

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

            await _classroomRepository.AddAsync(classroom, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Classroom created: {ClassroomId} - {Code}", classroom.Id, request.Code);

            return Result<Guid>.Success(classroom.Id, "Derslik başarıyla oluşturuldu.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating classroom");
            return Result<Guid>.Failure("Token yenileme sırasında bir hata oluştu. Hata: " + ex.Message);
        }
    }
}