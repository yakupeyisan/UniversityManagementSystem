using FluentValidation;

namespace UniversityMS.Application.Features.GradeFeature.Commands;

public class ReviewGradeObjectionCommandValidator : AbstractValidator<ReviewGradeObjectionCommand>
{
    public ReviewGradeObjectionCommandValidator()
    {
        RuleFor(x => x.ObjectionId)
            .NotEmpty().WithMessage("İtiraz ID boş olamaz.");

        RuleFor(x => x.IsApproved)
            .NotNull().WithMessage("Onay durumu belirtilmelidir.");

        RuleFor(x => x.NewScore)
            .NotNull().When(x => x.IsApproved)
            .WithMessage("Onay durumunda yeni not belirtilmelidir.")
            .GreaterThanOrEqualTo(0).When(x => x.NewScore.HasValue)
            .WithMessage("Yeni not 0'dan büyük veya eşit olmalıdır.")
            .LessThanOrEqualTo(100).When(x => x.NewScore.HasValue)
            .WithMessage("Yeni not 100'den küçük veya eşit olmalıdır.");

        RuleFor(x => x.ReviewNotes)
            .MaximumLength(1000).WithMessage("İnceleme notları en fazla 1000 karakter olabilir.")
            .NotEmpty().When(x => !x.IsApproved)
            .WithMessage("Reddetme durumunda açıklama belirtilmelidir.");
    }
}