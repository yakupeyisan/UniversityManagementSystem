using FluentValidation;

namespace UniversityMS.Application.Features.Students.Commands;

public class UpdateStudentStatusCommandValidator : AbstractValidator<UpdateStudentStatusCommand>
{
    public UpdateStudentStatusCommandValidator()
    {
        RuleFor(x => x.StudentId)
            .NotEmpty().WithMessage("Öğrenci ID gereklidir.");

        RuleFor(x => x.Status)
            .IsInEnum().WithMessage("Geçersiz durum.");
    }
}