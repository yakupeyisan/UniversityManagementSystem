using MediatR;
using Microsoft.Extensions.Logging;
using UniversityMS.Application.Common.Models;
using UniversityMS.Domain.Entities.AcademicAggregate;
using UniversityMS.Domain.Interfaces;

namespace UniversityMS.Application.Features.FacultyFeature.Commands;

public class DeleteFacultyCommandHandler : IRequestHandler<DeleteFacultyCommand, Result>
{
    private readonly IRepository<Faculty> _facultyRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<DeleteFacultyCommandHandler> _logger;

    public DeleteFacultyCommandHandler(
        IRepository<Faculty> facultyRepository,
        IUnitOfWork unitOfWork,
        ILogger<DeleteFacultyCommandHandler> logger)
    {
        _facultyRepository = facultyRepository;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Result> Handle(DeleteFacultyCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var faculty = await _facultyRepository.GetByIdAsync(request.Id, cancellationToken);
            if (faculty == null)
                return Result.Failure("Fakülte bulunamadı.");

            // Check if faculty has active departments
            if (faculty.Departments.Any(d => !d.IsDeleted))
                return Result.Failure("Aktif bölümleri olan fakülte silinemez.");

            faculty.Delete(request.DeletedBy);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Faculty deleted: {FacultyId}", request.Id);
            return Result.Success("Fakülte başarıyla silindi.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting faculty");
            return Result.Failure("Fakülte silinirken bir hata oluştu.", ex.Message);
        }
    }
}