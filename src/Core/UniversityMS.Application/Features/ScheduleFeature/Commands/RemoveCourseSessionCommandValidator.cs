using FluentValidation;

namespace UniversityMS.Application.Features.ScheduleFeature.Commands;

public class RemoveCourseSessionCommandValidator : AbstractValidator<RemoveCourseSessionCommand>
{
    public RemoveCourseSessionCommandValidator()
    {
        RuleFor(x => x.ScheduleId)
            .NotEmpty().WithMessage("Program ID gereklidir.");

        RuleFor(x => x.SessionId)
            .NotEmpty().WithMessage("Oturum ID gereklidir.");
    }
}