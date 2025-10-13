using FluentValidation;

namespace UniversityMS.Application.Features.Schedules.Commands;

public class PublishScheduleCommandValidator : AbstractValidator<PublishScheduleCommand>
{
    public PublishScheduleCommandValidator()
    {
        RuleFor(x => x.ScheduleId)
            .NotEmpty().WithMessage("Program ID gereklidir.");
    }
}