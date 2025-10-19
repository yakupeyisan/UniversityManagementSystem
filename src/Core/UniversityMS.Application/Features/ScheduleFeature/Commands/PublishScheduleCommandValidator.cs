using FluentValidation;

namespace UniversityMS.Application.Features.ScheduleFeature.Commands;

public class PublishScheduleCommandValidator : AbstractValidator<PublishScheduleCommand>
{
    public PublishScheduleCommandValidator()
    {
        RuleFor(x => x.ScheduleId)
            .NotEmpty().WithMessage("Program ID gereklidir.");
    }
}