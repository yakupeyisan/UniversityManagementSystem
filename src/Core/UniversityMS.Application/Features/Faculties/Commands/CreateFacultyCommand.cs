using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;
using UniversityMS.Application.Common.Models;
using UniversityMS.Domain.Entities.AcademicAggregate;
using UniversityMS.Domain.Interfaces;

namespace UniversityMS.Application.Features.Faculties.Commands;

public record CreateFacultyCommand(
    string Name,
    string Code,
    string? Description = null
) : IRequest<Result<Guid>>;

public class CreateFacultyCommandValidator : AbstractValidator<CreateFacultyCommand>
{
    public CreateFacultyCommandValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Code).NotEmpty().MaximumLength(20);
    }
}

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

            return Result.Success(faculty.Id, "Fakülte oluşturuldu.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating faculty");
            return Result.Failure<Guid>("Fakülte oluşturulamadı.");
        }
    }
}

public record UpdateFacultyCommand(
    Guid Id,
    string Name,
    string Code,
    string? Description
) : IRequest<Result<Guid>>;

public class UpdateFacultyCommandValidator : AbstractValidator<UpdateFacultyCommand>
{
    public UpdateFacultyCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Fakülte ID gereklidir.");

        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Fakülte adı boş olamaz.")
            .MaximumLength(200);

        RuleFor(x => x.Code)
            .NotEmpty().WithMessage("Fakülte kodu boş olamaz.")
            .MaximumLength(10);
    }
}

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
                return Result.Failure<Guid>("Fakülte bulunamadı.");

            var existingFaculty = await _facultyRepository.FirstOrDefaultAsync(
                f => f.Code == request.Code.Trim().ToUpperInvariant() && f.Id != request.Id,
                cancellationToken);

            if (existingFaculty != null)
                return Result.Failure<Guid>($"'{request.Code}' kodu başka bir fakülte tarafından kullanılıyor.");

            faculty.Update(request.Name, request.Code, request.Description);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Faculty updated: {FacultyId}", faculty.Id);
            return Result.Success(faculty.Id, "Fakülte başarıyla güncellendi.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating faculty");
            return Result.Failure<Guid>("Fakülte güncellenirken bir hata oluştu.", ex.Message);
        }
    }
}

public record DeleteFacultyCommand(Guid Id, string DeletedBy) : IRequest<Result>;

public class DeleteFacultyCommandValidator : AbstractValidator<DeleteFacultyCommand>
{
    public DeleteFacultyCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Fakülte ID gereklidir.");

        RuleFor(x => x.DeletedBy)
            .NotEmpty().WithMessage("Silme işlemini yapan kullanıcı bilgisi gereklidir.");
    }
}

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
