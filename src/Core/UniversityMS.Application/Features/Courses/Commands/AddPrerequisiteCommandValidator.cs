using FluentValidation;

namespace UniversityMS.Application.Features.Courses.Commands;

public class AddPrerequisiteCommandValidator : AbstractValidator<AddPrerequisiteCommand>
{
    public AddPrerequisiteCommandValidator()
    {
        RuleFor(x => x.CourseId)
            .NotEmpty().WithMessage("Ders ID gereklidir.");

        RuleFor(x => x.PrerequisiteCourseId)
            .NotEmpty().WithMessage("Ön koşul ders ID gereklidir.");

        RuleFor(x => x)
            .Must(x => x.CourseId != x.PrerequisiteCourseId)
            .WithMessage("Bir ders kendisinin ön koşulu olamaz.");
    }
}