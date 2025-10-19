using FluentValidation;

namespace UniversityMS.Application.Features.Grades.Commands;

public class CreateGradeObjectionCommandValidator : AbstractValidator<CreateGradeObjectionCommand>
{
    public CreateGradeObjectionCommandValidator()
    {
        RuleFor(x => x.GradeId)
            .NotEmpty()
            .WithMessage("Not ID gereklidir.");

        RuleFor(x => x.StudentId)
            .NotEmpty()
            .WithMessage("Öğrenci ID gereklidir.");

        RuleFor(x => x.CourseId)
            .NotEmpty()
            .WithMessage("Ders ID gereklidir.");

        RuleFor(x => x.Reason)
            .NotEmpty()
            .WithMessage("İtiraz nedeni boş olamaz.")
            .MinimumLength(10)
            .WithMessage("İtiraz nedeni en az 10 karakter olmalıdır.")
            .MaximumLength(1000)
            .WithMessage("İtiraz nedeni en fazla 1000 karakter olabilir.");
    }
}