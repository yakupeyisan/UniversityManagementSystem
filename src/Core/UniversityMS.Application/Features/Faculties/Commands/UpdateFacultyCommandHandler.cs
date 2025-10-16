using MediatR;
using Microsoft.Extensions.Logging;
using UniversityMS.Application.Common.Models;
using UniversityMS.Domain.Entities.AcademicAggregate;
using UniversityMS.Domain.Interfaces;

namespace UniversityMS.Application.Features.Faculties.Commands;

public class UpdateFacultyCommandHandler : IRequestHandler<UpdateFacultyCommand, Result<Guid>>
{
    private readonly IRepository<Faculty> _facultyRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<UpdateFacultyCommandHandler> _logger;

    public UpdateFacultyCommandHandler(
        IRepository<Faculty> facultyRepository,
        IUnitOfWork unitOfWork,
        ILogger<UpdateFacultyCommandHandler> logger)
    {
        _facultyRepository = facultyRepository;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Result<Guid>> Handle(UpdateFacultyCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var faculty = await _facultyRepository.GetByIdAsync(request.Id, cancellationToken);
            if (faculty == null)
                return Result<Guid>.Failure("Fakülte bulunamadı.");

            var existingFaculty = await _facultyRepository.FirstOrDefaultAsync(
                f => f.Code == request.Code.Trim().ToUpperInvariant() && f.Id != request.Id,
                cancellationToken);

            if (existingFaculty != null)
                return Result<Guid>.Failure($"'{request.Code}' kodu başka bir fakülte tarafından kullanılıyor.");

            faculty.Update(request.Name, request.Code, request.Description);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Faculty updated: {FacultyId}", faculty.Id);
            return Result<Guid>.Success(faculty.Id, "Fakülte başarıyla güncellendi.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating faculty");
            return Result<Guid>.Failure("Fakülte güncellenirken bir hata oluştu. Hata: "+ ex.Message);
        }
    }
}