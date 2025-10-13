using FluentValidation;

namespace UniversityMS.Application.Features.Schedules.Commands;

public class ActivateScheduleCommandValidator : AbstractValidator<ActivateScheduleCommand>
{
    public ActivateScheduleCommandValidator()
    {
        RuleFor(x => x.ScheduleId)
            .NotEmpty().WithMessage("Program ID gereklidir.");
    }
}