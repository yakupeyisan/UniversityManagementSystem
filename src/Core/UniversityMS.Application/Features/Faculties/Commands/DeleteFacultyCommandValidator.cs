using FluentValidation;

namespace UniversityMS.Application.Features.Faculties.Commands;

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