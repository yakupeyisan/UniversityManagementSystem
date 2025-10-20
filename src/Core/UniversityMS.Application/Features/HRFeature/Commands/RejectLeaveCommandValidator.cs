using FluentValidation;

namespace UniversityMS.Application.Features.HRFeature.Commands;

/// <summary>
/// İzin reddetme validator
/// </summary>
public class RejectLeaveCommandValidator : AbstractValidator<RejectLeaveCommand>
{
    public RejectLeaveCommandValidator()
    {
        // ========== İZİN ID KONTROLÜ ==========
        RuleFor(x => x.LeaveId)
            .NotEmpty().WithMessage("İzin ID'si boş olamaz.")
            .NotEqual(Guid.Empty).WithMessage("İzin ID'si geçerli olmalıdır.");

        // ========== RET NEDENİ KONTROLÜ ==========
        RuleFor(x => x.RejectionReason)
            .NotEmpty().WithMessage("Red nedeni boş olamaz.")
            .MinimumLength(5).WithMessage("Red nedeni en az 5 karakter olmalıdır.")
            .MaximumLength(500).WithMessage("Red nedeni en fazla 500 karakter olabilir.");
    }
}