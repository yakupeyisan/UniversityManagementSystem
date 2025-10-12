using FluentValidation;

namespace UniversityMS.Application.Features.Grades.Commands;

public class RemovePrerequisiteCommandValidator : AbstractValidator<RemovePrerequisiteCommand>
{
    public RemovePrerequisiteCommandValidator()
    {
        RuleFor(x => x.CourseId)
            .NotEmpty().WithMessage("Ders ID gereklidir.");

        RuleFor(x => x.PrerequisiteCourseId)
            .NotEmpty().WithMessage("Ön koşul ders ID gereklidir.");
    }
}