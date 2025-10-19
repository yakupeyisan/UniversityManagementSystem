using FluentValidation;

namespace UniversityMS.Application.Features.Courses.Commands;

public class UpdateCourseCommandValidator : AbstractValidator<UpdateCourseCommand>
{
    public UpdateCourseCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty()
            .WithMessage("Ders ID gereklidir.");

        RuleFor(x => x.Name)
            .NotEmpty()
            .WithMessage("Ders adı boş olamaz.")
            .MaximumLength(200)
            .WithMessage("Ders adı maksimum 200 karakter olabilir.");

        RuleFor(x => x.TheoreticalHours)
            .GreaterThanOrEqualTo(0)
            .WithMessage("Teorik saatler negatif olamaz.");

        RuleFor(x => x.PracticalHours)
            .GreaterThanOrEqualTo(0)
            .WithMessage("Uygulama saatleri negatif olamaz.");

        RuleFor(x => x.ECTS)
            .GreaterThan(0)
            .WithMessage("ECTS pozitif olmalıdır.");

        RuleFor(x => x.NationalCredit)
            .GreaterThanOrEqualTo(0)
            .WithMessage("Ulusal kredi negatif olamaz.");

        When(x => x.Semester.HasValue, () =>
        {
            RuleFor(x => x.Semester)
                .InclusiveBetween(1, 8)
                .WithMessage("Dönem 1-8 arasında olmalıdır.");
        });
    }
}