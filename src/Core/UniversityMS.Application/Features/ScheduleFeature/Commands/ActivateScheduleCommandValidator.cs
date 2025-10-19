using FluentValidation;

namespace UniversityMS.Application.Features.ScheduleFeature.Commands;

public class ActivateScheduleCommandValidator : AbstractValidator<ActivateScheduleCommand>
{
    public ActivateScheduleCommandValidator()
    {
        RuleFor(x => x.ScheduleId)
            .NotEmpty().WithMessage("Program ID gereklidir.");
    }
}