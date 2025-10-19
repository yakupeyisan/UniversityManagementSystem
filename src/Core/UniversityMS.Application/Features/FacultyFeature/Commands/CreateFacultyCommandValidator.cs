using FluentValidation;

namespace UniversityMS.Application.Features.FacultyFeature.Commands;

public class CreateFacultyCommandValidator : AbstractValidator<CreateFacultyCommand>
{
    public CreateFacultyCommandValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Code).NotEmpty().MaximumLength(20);
    }
}