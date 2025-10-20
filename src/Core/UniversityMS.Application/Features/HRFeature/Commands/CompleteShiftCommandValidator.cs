using FluentValidation;

namespace UniversityMS.Application.Features.HRFeature.Commands;

/// <summary>
/// Vardiya tamamlama validator
/// </summary>
public class CompleteShiftCommandValidator : AbstractValidator<CompleteShiftCommand>
{
    public CompleteShiftCommandValidator()
    {
        // ========== VARDIYA ID KONTROLÜ ==========
        RuleFor(x => x.ShiftId)
            .NotEmpty().WithMessage("Vardiya ID'si boş olamaz.")
            .NotEqual(Guid.Empty).WithMessage("Vardiya ID'si geçerli olmalıdır.");

        // ========== GERÇEK BİTİŞ SAATİ KONTROLÜ ==========
        RuleFor(x => x.ActualEndTime)
            .NotEmpty().WithMessage("Gerçek bitiş saati boş olamaz.");

        // ========== NOTLAR KONTROLÜ ==========
        RuleFor(x => x.Notes)
            .MaximumLength(500).When(x => !string.IsNullOrEmpty(x.Notes))
            .WithMessage("Vardiya notları en fazla 500 karakter olabilir.");
    }
}