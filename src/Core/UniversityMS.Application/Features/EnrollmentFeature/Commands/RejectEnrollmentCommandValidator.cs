using FluentValidation;

namespace UniversityMS.Application.Features.EnrollmentFeature.Commands;

public class RejectEnrollmentCommandValidator : AbstractValidator<RejectEnrollmentCommand>
{
    public RejectEnrollmentCommandValidator()
    {
        RuleFor(x => x.EnrollmentId)
            .NotEmpty().WithMessage("Kayıt ID boş olamaz.");

        RuleFor(x => x.RejectionReason)
            .NotEmpty().WithMessage("Red nedenini giriniz.")
            .MaximumLength(500).WithMessage("Red nedeni en fazla 500 karakter olabilir.");
    }
}