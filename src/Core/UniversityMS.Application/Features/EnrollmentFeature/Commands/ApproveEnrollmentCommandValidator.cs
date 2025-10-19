using FluentValidation;

namespace UniversityMS.Application.Features.EnrollmentFeature.Commands;

public class ApproveEnrollmentCommandValidator : AbstractValidator<ApproveEnrollmentCommand>
{
    public ApproveEnrollmentCommandValidator()
    {
        RuleFor(x => x.EnrollmentId)
            .NotEmpty().WithMessage("Kayıt ID boş olamaz.");

        RuleFor(x => x.ApprovedBy)
            .NotEmpty().WithMessage("Onaylayan kullanıcı ID boş olamaz.");
    }
}