using FluentValidation;

namespace UniversityMS.Application.Features.HRFeature.Commands;

/// <summary>
/// Sözleşme oluşturma validator
/// </summary>
public class CreateContractCommandValidator : AbstractValidator<CreateContractCommand>
{
    public CreateContractCommandValidator()
    {
        // ========== ÇALIŞANı KONTROLÜ ==========
        RuleFor(x => x.EmployeeId)
            .NotEmpty().WithMessage("Çalışan ID'si boş olamaz.")
            .NotEqual(Guid.Empty).WithMessage("Çalışan ID'si geçerli olmalıdır.");

        // ========== SÖZLEŞME NUMARASI KONTROLÜ ==========
        RuleFor(x => x.ContractNumber)
            .NotEmpty().WithMessage("Sözleşme numarası boş olamaz.")
            .MaximumLength(50).WithMessage("Sözleşme numarası en fazla 50 karakter olabilir.")
            .Matches(@"^[A-Z0-9-/]+$")
            .WithMessage("Sözleşme numarası geçersiz format. (Örnek: SK-2025-001)");

        // ========== SÖZLEŞME TİPİ KONTROLÜ ==========
        RuleFor(x => x.ContractType)
            .NotEmpty().WithMessage("Sözleşme türü seçiniz.")
            .Must(x => new[] { "Permanent", "FixedTerm", "PartTime", "Temporary" }.Contains(x))
            .WithMessage("Sözleşme türü: Permanent, FixedTerm, PartTime veya Temporary olmalıdır.");

        // ========== MAAŞ KONTROLÜ ==========
        RuleFor(x => x.BaseSalary)
            .GreaterThan(0).WithMessage("Temel maaş sıfırdan büyük olmalıdır.")
            .LessThanOrEqualTo(9999999).WithMessage("Temel maaş geçersiz.");

        // ========== TARİH KONTROLÜ ==========
        RuleFor(x => x.StartDate)
            .NotEmpty().WithMessage("Başlangıç tarihi boş olamaz.")
            .LessThanOrEqualTo(DateTime.UtcNow.AddDays(30))
            .WithMessage("Başlangıç tarihi 30 gün içinde olmalıdır.");

        RuleFor(x => x.EndDate)
            .GreaterThan(x => x.StartDate)
            .When(x => x.EndDate.HasValue && !string.IsNullOrEmpty(x.ContractType) && x.ContractType != "Permanent")
            .WithMessage("Bitiş tarihi, başlangıç tarihinden sonra olmalıdır. (Sürekli sözleşmeler için zorunlu değil)")
            .LessThanOrEqualTo(x => x.StartDate.AddYears(5))
            .When(x => x.EndDate.HasValue)
            .WithMessage("Sözleşme süresi 5 yıldan fazla olamaz.");

        // ========== KOŞULLAR KONTROLÜ ==========
        RuleFor(x => x.Terms)
            .NotEmpty().WithMessage("Sözleşme koşulları boş olamaz.")
            .MaximumLength(2000).WithMessage("Sözleşme koşulları en fazla 2000 karakter olabilir.");
    }
}
