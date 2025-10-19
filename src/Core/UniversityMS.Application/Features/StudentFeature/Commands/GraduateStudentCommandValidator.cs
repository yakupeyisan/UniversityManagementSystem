using FluentValidation;

namespace UniversityMS.Application.Features.StudentFeature.Commands;

public class GraduateStudentCommandValidator : AbstractValidator<GraduateStudentCommand>
{
    public GraduateStudentCommandValidator()
    {
        RuleFor(x => x.StudentId)
            .NotEmpty().WithMessage("Öğrenci ID gereklidir.");
    }
}