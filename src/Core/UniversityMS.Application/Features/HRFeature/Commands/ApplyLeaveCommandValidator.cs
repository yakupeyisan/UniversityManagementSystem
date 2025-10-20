using FluentValidation;
using UniversityMS.Application.Features.StaffFeature.Commands;

namespace UniversityMS.Application.Features.HRFeature.Commands;

/// <summary>
/// İzin başvurusu validator
/// </summary>
public class ApplyLeaveCommandValidator : AbstractValidator<ApplyLeaveCommand>
{
    public ApplyLeaveCommandValidator()
    {
        // ========== ÇALIŞANı KONTROLÜ ==========
        RuleFor(x => x.EmployeeId)
            .NotEmpty().WithMessage("Çalışan ID'si boş olamaz.")
            .NotEqual(Guid.Empty).WithMessage("Çalışan ID'si geçerli olmalıdır.");

        // ========== İZİN TİPİ KONTROLÜ ==========
        RuleFor(x => x.LeaveTypeId)
            .GreaterThan(0).WithMessage("Geçerli bir izin türü seçiniz.")
            .LessThan(100).WithMessage("İzin türü geçersiz.");

        // ========== TARİH KONTROLÜ ==========
        RuleFor(x => x.StartDate)
            .NotEmpty().WithMessage("Başlangıç tarihi boş olamaz.")
            .GreaterThanOrEqualTo(DateTime.UtcNow.Date)
            .WithMessage("Başlangıç tarihi geçmiş olamaz.");

        RuleFor(x => x.EndDate)
            .NotEmpty().WithMessage("Bitiş tarihi boş olamaz.")
            .GreaterThanOrEqualTo(x => x.StartDate)
            .WithMessage("Bitiş tarihi, başlangıç tarihinden sonra olmalıdır.");

        // ========== SÜRELİLİK KONTROLÜ ==========
        RuleFor(x => new { x.StartDate, x.EndDate })
            .Must(x =>
            {
                var duration = (x.EndDate.Date - x.StartDate.Date).Days + 1;
                return duration >= 1 && duration <= 365;
            })
            .WithMessage("İzin süresi 1 gün ile 1 yıl arasında olmalıdır.");

        // ========== AÇIKLAMA KONTROLÜ ==========
        RuleFor(x => x.Reason)
            .NotEmpty().WithMessage("İzin nedeni boş olamaz.")
            .MaximumLength(500).WithMessage("İzin nedeni en fazla 500 karakter olabilir.");
    }
}
