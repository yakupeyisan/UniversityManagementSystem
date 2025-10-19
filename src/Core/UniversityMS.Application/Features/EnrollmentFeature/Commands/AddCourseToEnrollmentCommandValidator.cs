using FluentValidation;

namespace UniversityMS.Application.Features.EnrollmentFeature.Commands;

public class AddCourseToEnrollmentCommandValidator : AbstractValidator<AddCourseToEnrollmentCommand>
{
    public AddCourseToEnrollmentCommandValidator()
    {
        RuleFor(x => x.EnrollmentId)
            .NotEmpty().WithMessage("Kayıt ID boş olamaz.");

        RuleFor(x => x.CourseId)
            .NotEmpty().WithMessage("Ders ID boş olamaz.");
    }
}