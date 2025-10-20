using FluentValidation;
using UniversityMS.Application.Features.StaffFeature.Commands;

namespace UniversityMS.Application.Features.HRFeature.Commands;

/// <summary>
/// İzin Talep Command Validator'ı
/// </summary>
public class ApplyLeaveCommandValidator : AbstractValidator<ApplyLeaveCommand>
{
    public ApplyLeaveCommandValidator()
    {
        // ========== EMPLOYEE ID VALIDASYONU ==========
        RuleFor(x => x.EmployeeId)
            .NotEmpty().WithMessage("Çalışan ID boş olamaz.")
            .NotEqual(Guid.Empty).WithMessage("Geçerli bir çalışan ID'si giriniz.");

        // ========== LEAVE TYPE VALIDASYONU ==========
        RuleFor(x => x.LeaveTypeId)
            .GreaterThan(0).WithMessage("İzin tipi ID'si geçerli olmalıdır.")
            .LessThanOrEqualTo(5).WithMessage("Geçersiz izin tipi.");

        // ========== TARIH VALIDASYONU ==========
        RuleFor(x => x.StartDate)
            .NotEmpty().WithMessage("Başlangıç tarihi boş olamaz.")
            .GreaterThanOrEqualTo(DateTime.UtcNow.Date).WithMessage("Başlangıç tarihi bugüne eşit veya sonra olmalıdır.")
            .LessThanOrEqualTo(DateTime.UtcNow.AddYears(1)).WithMessage("Başlangıç tarihi 1 yıl ileri olamaz.");

        RuleFor(x => x.EndDate)
            .NotEmpty().WithMessage("Bitiş tarihi boş olamaz.")
            .GreaterThanOrEqualTo(x => x.StartDate).WithMessage("Bitiş tarihi başlangıç tarihine eşit veya sonra olmalıdır.")
            .LessThanOrEqualTo(DateTime.UtcNow.AddYears(1)).WithMessage("Bitiş tarihi 1 yıl ileri olamaz.");

        // ========== TARİH ARALIĞI VALIDASYONU ==========
        RuleFor(x => x)
            .Custom((request, context) =>
            {
                var duration = (request.EndDate.Date - request.StartDate.Date).Days + 1;

                // Maximum 30 gün izin talep edilebilir
                if (duration > 30)
                {
                    context.AddFailure(
                        "Bir seferde maksimum 30 gün izin talep edilebilir.");
                }

                // Minimum 1 gün
                if (duration < 1)
                {
                    context.AddFailure(
                        "Minimum 1 gün izin talep edilmelidir.");
                }
            });

        // ========== REASON VALIDASYONU ==========
        RuleFor(x => x.Reason)
            .NotEmpty().WithMessage("İzin nedeni boş olamaz.")
            .MinimumLength(5).WithMessage("İzin nedeni en az 5 karakter olmalıdır.")
            .MaximumLength(500).WithMessage("İzin nedeni maksimum 500 karakter olabilir.")
            .Matches(@"^[a-zA-ZçğıöşüÇĞİÖŞÜ\s0-9.,;:-]*$")
            .WithMessage("İzin nedeni geçerli karakterler içermelidir.");
    }
}
