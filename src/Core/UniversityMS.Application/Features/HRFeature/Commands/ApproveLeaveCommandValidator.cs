using FluentValidation;

namespace UniversityMS.Application.Features.HRFeature.Commands;

/// <summary>
/// İzin onaylama validator
/// </summary>
public class ApproveLeaveCommandValidator : AbstractValidator<ApproveLeaveCommand>
{
    public ApproveLeaveCommandValidator()
    {
        // ========== İZİN ID KONTROLÜ ==========
        RuleFor(x => x.LeaveId)
            .NotEmpty().WithMessage("İzin ID'si boş olamaz.")
            .NotEqual(Guid.Empty).WithMessage("İzin ID'si geçerli olmalıdır.");

        // ========== ONAY NOTU KONTROLÜ ==========
        RuleFor(x => x.ApprovalNotes)
            .MaximumLength(500).When(x => !string.IsNullOrEmpty(x.ApprovalNotes))
            .WithMessage("Onay notu en fazla 500 karakter olabilir.");
    }
}
