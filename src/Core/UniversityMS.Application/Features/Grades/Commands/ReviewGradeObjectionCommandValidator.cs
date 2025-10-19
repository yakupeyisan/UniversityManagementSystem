using FluentValidation;

namespace UniversityMS.Application.Features.Grades.Commands;

public class ReviewGradeObjectionCommandValidator : AbstractValidator<ReviewGradeObjectionCommand>
{
    public ReviewGradeObjectionCommandValidator()
    {
        RuleFor(x => x.ObjectionId)
            .NotEmpty()
            .WithMessage("İtiraz ID gereklidir.");

        RuleFor(x => x.ReviewedBy)
            .NotEmpty()
            .WithMessage("İnceleme yapan ID gereklidir.");

        // ✅ NEW: If approved, NewScore is required
        RuleFor(x => x.NewScore)
            .NotNull()
            .WithMessage("Yeni not belirtilmelidir.")
            .When(x => x.IsApproved)
            .InclusiveBetween(0, 100)
            .WithMessage("Not 0-100 arasında olmalıdır.");

        // ✅ NEW: ReviewNotes can be added in either case
        RuleFor(x => x.ReviewNotes)
            .MaximumLength(1000)
            .WithMessage("Notlar en fazla 1000 karakter olabilir.")
            .When(x => !string.IsNullOrEmpty(x.ReviewNotes));
    }
}