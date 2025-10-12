using FluentValidation;

namespace UniversityMS.Application.Features.Enrollments.Commands;

public class SubmitEnrollmentCommandValidator : AbstractValidator<SubmitEnrollmentCommand>
{
    public SubmitEnrollmentCommandValidator()
    {
        RuleFor(x => x.EnrollmentId)
            .NotEmpty().WithMessage("Kayıt ID gereklidir.");
    }
}