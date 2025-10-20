using FluentValidation;

namespace UniversityMS.Application.Features.PayrollFeature.Commands;

/// <summary>
/// Gelir Vergisi Hesaplama Command Validator'ı
/// </summary>
public class CalculateTaxCommandValidator : AbstractValidator<CalculateTaxCommand>
{
    public CalculateTaxCommandValidator()
    {
        RuleFor(x => x.PayrollId)
            .NotEmpty().WithMessage("Bordro ID boş olamaz.")
            .NotEqual(Guid.Empty).WithMessage("Bordro ID geçerli olmalıdır.");

        RuleFor(x => x.GrossSalary)
            .GreaterThan(0).WithMessage("Brüt maaş sıfırdan büyük olmalıdır.")
            .LessThanOrEqualTo(999999999).WithMessage("Brüt maaş geçersiz.");

        RuleFor(x => x.SGKDeductions)
            .GreaterThanOrEqualTo(0).WithMessage("SGK kesintileri negatif olamaz.")
            .LessThan(x => x.GrossSalary).WithMessage("SGK kesintileri brüt maaştan az olmalıdır.");

        RuleFor(x => x.TaxDiscount)
            .GreaterThanOrEqualTo(0).When(x => x.TaxDiscount.HasValue).WithMessage("Vergi indirimi negatif olamaz.")
            .LessThanOrEqualTo(100).When(x => x.TaxDiscount.HasValue).WithMessage("Vergi indirimi %100'den fazla olamaz.");

        RuleFor(x => x.TaxYear)
            .GreaterThanOrEqualTo(2020).When(x => x.TaxYear.HasValue).WithMessage("Vergi yılı 2020'den eski olamaz.")
            .LessThanOrEqualTo(DateTime.UtcNow.Year + 1).When(x => x.TaxYear.HasValue)
            .WithMessage("Gelecek yıl için vergi hesaplama yapılamaz.");

        RuleFor(x => x.TaxMonth)
            .GreaterThanOrEqualTo(1).When(x => x.TaxMonth.HasValue).WithMessage("Ay 1'den az olamaz.")
            .LessThanOrEqualTo(12).When(x => x.TaxMonth.HasValue).WithMessage("Ay 12'den fazla olamaz.");
    }
}