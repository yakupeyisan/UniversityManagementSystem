using FluentValidation;

namespace UniversityMS.Application.Features.EnrollmentFeature.Commands;

public class SubmitEnrollmentCommandValidator : AbstractValidator<SubmitEnrollmentCommand>
{
    public SubmitEnrollmentCommandValidator()
    {
        RuleFor(x => x.EnrollmentId)
            .NotEmpty().WithMessage("Kayıt ID boş olamaz.");
    }
}