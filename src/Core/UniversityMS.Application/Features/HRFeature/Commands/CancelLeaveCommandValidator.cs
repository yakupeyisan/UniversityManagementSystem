using FluentValidation;

namespace UniversityMS.Application.Features.HRFeature.Commands;

/// <summary>
/// İzin iptal validator
/// </summary>
public class CancelLeaveCommandValidator : AbstractValidator<CancelLeaveCommand>
{
    public CancelLeaveCommandValidator()
    {
        // ========== İZİN ID KONTROLÜ ==========
        RuleFor(x => x.LeaveId)
            .NotEmpty().WithMessage("İzin ID'si boş olamaz.")
            .NotEqual(Guid.Empty).WithMessage("İzin ID'si geçerli olmalıdır.");

        // ========== İPTAL NEDENİ KONTROLÜ ==========
        RuleFor(x => x.CancellationReason)
            .NotEmpty().WithMessage("İptal nedeni boş olamaz.")
            .MinimumLength(5).WithMessage("İptal nedeni en az 5 karakter olmalıdır.")
            .MaximumLength(500).WithMessage("İptal nedeni en fazla 500 karakter olabilir.");
    }
}