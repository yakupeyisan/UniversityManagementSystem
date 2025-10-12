using FluentValidation;

namespace UniversityMS.Application.Features.Students.Commands;

public class FreezeStudentCommandValidator : AbstractValidator<FreezeStudentCommand>
{
    public FreezeStudentCommandValidator()
    {
        RuleFor(x => x.StudentId)
            .NotEmpty().WithMessage("Öğrenci ID gereklidir.");
    }
}