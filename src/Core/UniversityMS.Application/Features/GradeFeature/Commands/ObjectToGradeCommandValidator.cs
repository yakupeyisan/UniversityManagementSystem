using FluentValidation;

namespace UniversityMS.Application.Features.GradeFeature.Commands;

public class ObjectToGradeCommandValidator : AbstractValidator<ObjectToGradeCommand>
{
    public ObjectToGradeCommandValidator()
    {
        RuleFor(x => x.GradeId)
            .NotEmpty().WithMessage("Not ID boş olamaz.");

        RuleFor(x => x.StudentId)
            .NotEmpty().WithMessage("Öğrenci ID boş olamaz.");

        RuleFor(x => x.Reason)
            .NotEmpty().WithMessage("İtiraz nedeni boş olamaz.")
            .MaximumLength(200).WithMessage("İtiraz nedeni en fazla 200 karakter olabilir.");

        RuleFor(x => x.Description)
            .MaximumLength(1000).WithMessage("Açıklama en fazla 1000 karakter olabilir.");
    }
}