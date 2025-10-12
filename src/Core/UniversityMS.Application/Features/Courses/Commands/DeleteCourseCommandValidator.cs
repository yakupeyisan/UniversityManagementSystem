using FluentValidation;

namespace UniversityMS.Application.Features.Courses.Commands;

public class DeleteCourseCommandValidator : AbstractValidator<DeleteCourseCommand>
{
    public DeleteCourseCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Ders ID gereklidir.");
    }
}