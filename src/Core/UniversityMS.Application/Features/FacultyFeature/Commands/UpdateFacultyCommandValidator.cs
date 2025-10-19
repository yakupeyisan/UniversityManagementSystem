using FluentValidation;

namespace UniversityMS.Application.Features.FacultyFeature.Commands;

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