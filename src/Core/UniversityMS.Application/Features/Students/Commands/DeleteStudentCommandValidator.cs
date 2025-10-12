using FluentValidation;

namespace UniversityMS.Application.Features.Students.Commands;

public class DeleteStudentCommandValidator : AbstractValidator<DeleteStudentCommand>
{
    public DeleteStudentCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Öğrenci ID gereklidir.");
    }
}