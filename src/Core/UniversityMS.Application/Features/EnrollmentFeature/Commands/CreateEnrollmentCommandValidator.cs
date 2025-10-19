using FluentValidation;

namespace UniversityMS.Application.Features.EnrollmentFeature.Commands;

public class CreateEnrollmentCommandValidator : AbstractValidator<CreateEnrollmentCommand>
{
    public CreateEnrollmentCommandValidator()
    {
        RuleFor(x => x.StudentId)
            .NotEmpty().WithMessage("Öğrenci ID boş olamaz.");

        RuleFor(x => x.AcademicYear)
            .NotEmpty().WithMessage("Akademik yıl boş olamaz.")
            .Matches(@"^\d{4}-\d{4}$").WithMessage("Akademik yıl YYYY-YYYY formatında olmalıdır.");

        RuleFor(x => x.Semester)
            .InclusiveBetween(1, 2).WithMessage("Yarıyıl 1 veya 2 olmalıdır.");
    }
}