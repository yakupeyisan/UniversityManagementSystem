using FluentValidation;

namespace UniversityMS.Application.Features.StudentFeature.Commands;

public class UpdateStudentStatusCommandValidator : AbstractValidator<UpdateStudentStatusCommand>
{
    public UpdateStudentStatusCommandValidator()
    {
        RuleFor(x => x.StudentId)
            .NotEmpty().WithMessage("Öğrenci ID boş olamaz.");
    }
}