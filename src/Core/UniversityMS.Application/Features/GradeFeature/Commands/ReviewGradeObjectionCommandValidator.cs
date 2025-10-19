using FluentValidation;

namespace UniversityMS.Application.Features.GradeFeature.Commands;

public class ReviewGradeObjectionCommandValidator : AbstractValidator<ReviewGradeObjectionCommand>
{
    public ReviewGradeObjectionCommandValidator()
    {
        RuleFor(x => x.ObjectionId)
            .NotEmpty().WithMessage("İtiraz ID boş olamaz.");

        RuleFor(x => x.ReviewNotes)
            .MaximumLength(1000).WithMessage("İnceleme notları en fazla 1000 karakter olabilir.");
    }
}