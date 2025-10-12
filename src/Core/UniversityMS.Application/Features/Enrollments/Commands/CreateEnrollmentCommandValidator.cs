using FluentValidation;

namespace UniversityMS.Application.Features.Enrollments.Commands;

public class CreateEnrollmentCommandValidator : AbstractValidator<CreateEnrollmentCommand>
{
    public CreateEnrollmentCommandValidator()
    {
        RuleFor(x => x.StudentId)
            .NotEmpty().WithMessage("Öğrenci ID gereklidir.");

        RuleFor(x => x.AcademicYear)
            .NotEmpty().WithMessage("Akademik yıl gereklidir.")
            .Matches(@"^\d{4}-\d{4}$").WithMessage("Akademik yıl formatı: YYYY-YYYY");

        RuleFor(x => x.Semester)
            .InclusiveBetween(1, 2).WithMessage("Dönem 1 (Güz) veya 2 (Bahar) olmalıdır.");
    }
}