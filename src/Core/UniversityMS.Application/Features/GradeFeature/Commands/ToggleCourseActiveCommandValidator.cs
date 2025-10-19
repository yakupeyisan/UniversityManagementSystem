using FluentValidation;

namespace UniversityMS.Application.Features.GradeFeature.Commands;

public class ToggleCourseActiveCommandValidator : AbstractValidator<ToggleCourseActiveCommand>
{
    public ToggleCourseActiveCommandValidator()
    {
        RuleFor(x => x.CourseId)
            .NotEmpty().WithMessage("Ders ID gereklidir.");
    }
}