using FluentValidation;

namespace UniversityMS.Application.Features.Enrollments.Commands;

public class RejectEnrollmentCommandValidator : AbstractValidator<RejectEnrollmentCommand>
{
    public RejectEnrollmentCommandValidator()
    {
        RuleFor(x => x.EnrollmentId)
            .NotEmpty().WithMessage("Kayıt ID gereklidir.");

        RuleFor(x => x.AdvisorId)
            .NotEmpty().WithMessage("Danışman ID gereklidir.");

        RuleFor(x => x.Reason)
            .NotEmpty().WithMessage("Red nedeni belirtilmelidir.")
            .MaximumLength(500);
    }
}