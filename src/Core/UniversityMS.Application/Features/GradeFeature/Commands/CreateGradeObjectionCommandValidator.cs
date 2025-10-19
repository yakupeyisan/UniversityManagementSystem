using FluentValidation;

namespace UniversityMS.Application.Features.GradeFeature.Commands;

public class CreateGradeObjectionCommandValidator : AbstractValidator<CreateGradeObjectionCommand>
{
    public CreateGradeObjectionCommandValidator()
    {
        RuleFor(x => x.GradeId)
            .NotEmpty().WithMessage("Not ID boş olamaz.");

        RuleFor(x => x.StudentId)
            .NotEmpty().WithMessage("Öğrenci ID boş olamaz.");

        RuleFor(x => x.Reason)
            .NotEmpty().WithMessage("İtiraz nedeni boş olamaz.")
            .MaximumLength(1000).WithMessage("İtiraz nedeni en fazla 1000 karakter olabilir.");
    }
}