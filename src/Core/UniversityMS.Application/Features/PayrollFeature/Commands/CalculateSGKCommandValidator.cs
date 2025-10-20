using FluentValidation;

namespace UniversityMS.Application.Features.PayrollFeature.Commands;

/// <summary>
/// SGK Hesaplama Command Validator'ı
/// </summary>
public class CalculateSGKCommandValidator : AbstractValidator<CalculateSGKCommand>
{
    public CalculateSGKCommandValidator()
    {
        RuleFor(x => x.PayrollId)
            .NotEmpty().WithMessage("Bordro ID boş olamaz.")
            .NotEqual(Guid.Empty).WithMessage("Bordro ID geçerli olmalıdır.");

        RuleFor(x => x.GrossSalary)
            .GreaterThan(0).WithMessage("Brüt maaş sıfırdan büyük olmalıdır.")
            .LessThanOrEqualTo(999999999).WithMessage("Brüt maaş geçersiz.");

        RuleFor(x => x.PremiumDays)
            .GreaterThan(0).WithMessage("Prim günü 0'dan büyük olmalıdır.")
            .LessThanOrEqualTo(31).WithMessage("Prim günü 31'den fazla olamaz.");

        RuleFor(x => x.CalculationYear)
            .GreaterThanOrEqualTo(2020).When(x => x.CalculationYear.HasValue).WithMessage("Hesaplama yılı 2020'den eski olamaz.")
            .LessThanOrEqualTo(DateTime.UtcNow.Year + 1).When(x => x.CalculationYear.HasValue)
            .WithMessage("Gelecek yıl için SGK hesaplama yapılamaz.");

        RuleFor(x => x.CalculationMonth)
            .GreaterThanOrEqualTo(1).When(x => x.CalculationMonth.HasValue).WithMessage("Ay 1'den az olamaz.")
            .LessThanOrEqualTo(12).When(x => x.CalculationMonth.HasValue).WithMessage("Ay 12'den fazla olamaz.");
    }
}