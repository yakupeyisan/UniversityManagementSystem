using FluentValidation;

namespace UniversityMS.Application.Features.Enrollments.Commands;

public class RemoveCourseFromEnrollmentCommandValidator : AbstractValidator<RemoveCourseFromEnrollmentCommand>
{
    public RemoveCourseFromEnrollmentCommandValidator()
    {
        RuleFor(x => x.EnrollmentId)
            .NotEmpty().WithMessage("Kayıt ID gereklidir.");

        RuleFor(x => x.CourseId)
            .NotEmpty().WithMessage("Ders ID gereklidir.");
    }
}