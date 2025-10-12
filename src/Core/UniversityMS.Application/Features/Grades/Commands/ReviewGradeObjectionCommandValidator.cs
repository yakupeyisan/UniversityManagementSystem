using FluentValidation;

namespace UniversityMS.Application.Features.Grades.Commands;

public class ReviewGradeObjectionCommandValidator : AbstractValidator<ReviewGradeObjectionCommand>
{
    public ReviewGradeObjectionCommandValidator()
    {
        RuleFor(x => x.ObjectionId).NotEmpty();
        RuleFor(x => x.ReviewedBy).NotEmpty();
        RuleFor(x => x.NewScore)
            .InclusiveBetween(0, 100)
            .When(x => x.IsApproved && x.NewScore.HasValue);
    }
}