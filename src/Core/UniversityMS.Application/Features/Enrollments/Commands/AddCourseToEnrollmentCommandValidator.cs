using FluentValidation;

namespace UniversityMS.Application.Features.Enrollments.Commands;

public class AddCourseToEnrollmentCommandValidator : AbstractValidator<AddCourseToEnrollmentCommand>
{
    public AddCourseToEnrollmentCommandValidator()
    {
        RuleFor(x => x.EnrollmentId)
            .NotEmpty().WithMessage("Kayıt ID gereklidir.");

        RuleFor(x => x.CourseId)
            .NotEmpty().WithMessage("Ders ID gereklidir.");
    }
}