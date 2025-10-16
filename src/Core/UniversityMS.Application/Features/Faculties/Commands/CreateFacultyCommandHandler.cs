using MediatR;
using Microsoft.Extensions.Logging;
using UniversityMS.Application.Common.Models;
using UniversityMS.Domain.Entities.AcademicAggregate;
using UniversityMS.Domain.Interfaces;

namespace UniversityMS.Application.Features.Faculties.Commands;

public class CreateFacultyCommandHandler : IRequestHandler<CreateFacultyCommand, Result<Guid>>
{
    private readonly IRepository<Faculty> _facultyRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<CreateFacultyCommandHandler> _logger;

    public CreateFacultyCommandHandler(
        IRepository<Faculty> facultyRepository,
        IUnitOfWork unitOfWork,
        ILogger<CreateFacultyCommandHandler> logger)
    {
        _facultyRepository = facultyRepository;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Result<Guid>> Handle(CreateFacultyCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var faculty = Faculty.Create(request.Name, request.Code, request.Description);

            await _facultyRepository.AddAsync(faculty, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result<Guid>.Success(faculty.Id, "Fakülte oluşturuldu.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating faculty");
            return Result<Guid>.Failure("Fakülte oluşturulamadı.");
        }
    }
}