using FluentValidation;
using MediatR;
using UniversityMS.Application.Common.Models;
using UniversityMS.Application.Features.HRFeature.DTOs;

namespace UniversityMS.Application.Features.HRFeature.Commands;

/// <summary>
/// Onaylanmış izin talebini iptal et
/// </summary>
public record CancelLeaveCommand(
    Guid LeaveId,
    string CancellationReason
) : IRequest<Result<LeaveDetailDto>>;

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
