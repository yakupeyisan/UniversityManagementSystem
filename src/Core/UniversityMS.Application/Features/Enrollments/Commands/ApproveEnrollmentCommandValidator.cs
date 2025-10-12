using FluentValidation;

namespace UniversityMS.Application.Features.Enrollments.Commands;

public class ApproveEnrollmentCommandValidator : AbstractValidator<ApproveEnrollmentCommand>
{
    public ApproveEnrollmentCommandValidator()
    {
        RuleFor(x => x.EnrollmentId)
            .NotEmpty().WithMessage("Kayıt ID gereklidir.");

        RuleFor(x => x.AdvisorId)
            .NotEmpty().WithMessage("Danışman ID gereklidir.");
    }
}