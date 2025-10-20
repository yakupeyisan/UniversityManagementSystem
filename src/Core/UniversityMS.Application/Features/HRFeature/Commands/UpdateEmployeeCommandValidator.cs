using FluentValidation;

namespace UniversityMS.Application.Features.HRFeature.Commands;

/// <summary>
/// Çalışan güncelleme command'ı validator'ı
/// Girdi validasyonlarını yapar
/// </summary>
public class UpdateEmployeeCommandValidator : AbstractValidator<UpdateEmployeeCommand>
{
    public UpdateEmployeeCommandValidator()
    {
        // ========== ÇALIŞANı ID KONTROLÜ ==========
        RuleFor(x => x.EmployeeId)
            .NotEmpty().WithMessage("Çalışan ID'si boş olamaz.")
            .NotEqual(Guid.Empty).WithMessage("Çalışan ID'si geçerli olmalıdır.");

        // ========== İŞ ÜNVANI KONTROLÜ (Opsiyonel) ==========
        RuleFor(x => x.JobTitle)
            .MaximumLength(100).When(x => !string.IsNullOrEmpty(x.JobTitle))
            .WithMessage("İş ünvanı en fazla 100 karakter olmalıdır.");

        // ========== DEPARTMAN ID KONTROLÜ (Opsiyonel) ==========
        RuleFor(x => x.DepartmentId)
            .Must(x => x == null || x != Guid.Empty)
            .When(x => x.DepartmentId.HasValue)
            .WithMessage("Geçerli bir departman ID'si giriniz.");

        // ========== MAAŞ KONTROLÜ (Opsiyonel) ==========
        RuleFor(x => x.BaseSalary)
            .GreaterThan(0).When(x => x.BaseSalary.HasValue)
            .WithMessage("Temel maaş 0'dan büyük olmalıdır.")
            .LessThanOrEqualTo(999999999).When(x => x.BaseSalary.HasValue)
            .WithMessage("Temel maaş geçersiz.");

        // ========== ÇALIŞıMA SAATİ KONTROLÜ (Tüm alanlar dolu olmalı) ==========
        RuleFor(x => new { x.StartTime, x.EndTime, x.WeeklyHours })
            .Must(x =>
            {
                var hasStart = x.StartTime.HasValue;
                var hasEnd = x.EndTime.HasValue;
                var hasWeekly = x.WeeklyHours.HasValue;

                // Ya hepsi dolu ya hiçbiri dolu değil
                if (hasStart || hasEnd || hasWeekly)
                    return hasStart && hasEnd && hasWeekly;

                return true;
            })
            .WithMessage("Çalışma saatleri tamamen belirtilmelidir (Başlangıç, Bitiş ve Haftalık Saat).");

        RuleFor(x => x.EndTime)
            .GreaterThan(x => x.StartTime)
            .When(x => x.StartTime.HasValue && x.EndTime.HasValue)
            .WithMessage("Bitiş saati başlangıç saatinden sonra olmalıdır.");

        RuleFor(x => x.WeeklyHours)
            .GreaterThan(0).When(x => x.WeeklyHours.HasValue)
            .WithMessage("Haftalık çalışma saati 0'dan büyük olmalıdır.")
            .LessThanOrEqualTo(168).When(x => x.WeeklyHours.HasValue)
            .WithMessage("Haftalık çalışma saati 168'den fazla olamaz.");

        // ========== NOTLAR KONTROLÜ (Opsiyonel) ==========
        RuleFor(x => x.Notes)
            .MaximumLength(500).When(x => !string.IsNullOrEmpty(x.Notes))
            .WithMessage("Notlar en fazla 500 karakter olabilir.");
    }
}