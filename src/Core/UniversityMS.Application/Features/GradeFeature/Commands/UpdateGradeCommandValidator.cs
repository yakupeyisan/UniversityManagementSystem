using FluentValidation;

namespace UniversityMS.Application.Features.GradeFeature.Commands;

public class UpdateGradeCommandValidator : AbstractValidator<UpdateGradeCommand>
{
    public UpdateGradeCommandValidator()
    {
        RuleFor(x => x.GradeId).NotEmpty();
        RuleFor(x => x.NumericScore).InclusiveBetween(0, 100);
    }
}