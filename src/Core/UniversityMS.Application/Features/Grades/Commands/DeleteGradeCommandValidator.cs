using FluentValidation;

namespace UniversityMS.Application.Features.Grades.Commands;

public class DeleteGradeCommandValidator : AbstractValidator<DeleteGradeCommand>
{
    public DeleteGradeCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Not ID gereklidir.");
    }
}