using FluentValidation;

namespace UniversityMS.Application.Features.Courses.Commands;

public class UpdateCourseCommandValidator : AbstractValidator<UpdateCourseCommand>
{
    public UpdateCourseCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Ders ID gereklidir.");

        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Ders adı boş olamaz.")
            .MaximumLength(200);

        RuleFor(x => x.TheoreticalHours)
            .GreaterThanOrEqualTo(0).WithMessage("Teorik saat negatif olamaz.");

        RuleFor(x => x.PracticalHours)
            .GreaterThanOrEqualTo(0).WithMessage("Uygulama saati negatif olamaz.");

        RuleFor(x => x.ECTS)
            .GreaterThan(0).WithMessage("ECTS en az 1 olmalıdır.");

        RuleFor(x => x.Semester)
            .InclusiveBetween(1, 8).When(x => x.Semester.HasValue)
            .WithMessage("Dönem 1-8 arasında olmalıdır.");
    }
}