using FluentValidation;

namespace UniversityMS.Application.Features.Grades.Commands;

public class ObjectToGradeCommandValidator : AbstractValidator<ObjectToGradeCommand>
{
    public ObjectToGradeCommandValidator()
    {
        RuleFor(x => x.GradeId).NotEmpty();
        RuleFor(x => x.StudentId).NotEmpty();
        RuleFor(x => x.Reason).NotEmpty().MinimumLength(20).MaximumLength(1000);
    }
}