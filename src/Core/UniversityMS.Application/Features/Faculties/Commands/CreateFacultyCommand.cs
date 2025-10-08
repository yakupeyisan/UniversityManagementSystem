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
    string? Description
) : IRequest<Result<Guid>>;
public class CreateFacultyCommandValidator : AbstractValidator<CreateFacultyCommand>
{
    public CreateFacultyCommandValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Fakülte adı boş olamaz.")
            .MaximumLength(200).WithMessage("Fakülte adı en fazla 200 karakter olabilir.");

        RuleFor(x => x.Code)
            .NotEmpty().WithMessage("Fakülte kodu boş olamaz.")
            .MaximumLength(10).WithMessage("Fakülte kodu en fazla 10 karakter olabilir.");
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
            // Check if code exists
            var existingFaculty = await _facultyRepository.FirstOrDefaultAsync(
                f => f.Code == request.Code.Trim().ToUpperInvariant(),
                cancellationToken);

            if (existingFaculty != null)
                return Result.Failure<Guid>($"'{request.Code}' kodu zaten kullanımda.");

            var faculty = Faculty.Create(request.Name, request.Code, request.Description);

            await _facultyRepository.AddAsync(faculty, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Faculty created: {FacultyId} - {FacultyName}", faculty.Id, faculty.Name);

            return Result.Success(faculty.Id, "Fakülte başarıyla oluşturuldu.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating faculty");
            return Result.Failure<Guid>("Fakülte oluşturulurken bir hata oluştu.", ex.Message);
        }
    }
}
