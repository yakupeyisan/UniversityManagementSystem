using FluentValidation;

namespace UniversityMS.Application.Features.CourseFeature.Commands;

public class CreateCourseCommandValidator : AbstractValidator<CreateCourseCommand>
{
    public CreateCourseCommandValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Ders adı boş olamaz.")
            .MaximumLength(200);

        RuleFor(x => x.Code)
            .NotEmpty().WithMessage("Ders kodu boş olamaz.")
            .MaximumLength(20);

        RuleFor(x => x.DepartmentId)
            .NotEmpty().WithMessage("Bölüm seçilmelidir.");

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