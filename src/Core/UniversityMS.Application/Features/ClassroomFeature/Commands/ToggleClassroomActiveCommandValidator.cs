using FluentValidation;

namespace UniversityMS.Application.Features.ClassroomFeature.Commands;

public class ToggleClassroomActiveCommandValidator : AbstractValidator<ToggleClassroomActiveCommand>
{
    public ToggleClassroomActiveCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Derslik ID gereklidir.")
            .NotEqual(Guid.Empty).WithMessage("Derslik ID boş GUID olamaz.");
    }
}