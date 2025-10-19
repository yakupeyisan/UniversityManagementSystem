using FluentValidation;

namespace UniversityMS.Application.Features.StudentFeature.Commands;

public class FreezeStudentCommandValidator : AbstractValidator<FreezeStudentCommand>
{
    public FreezeStudentCommandValidator()
    {
        RuleFor(x => x.StudentId)
            .NotEmpty().WithMessage("Öğrenci ID gereklidir.");
    }
}