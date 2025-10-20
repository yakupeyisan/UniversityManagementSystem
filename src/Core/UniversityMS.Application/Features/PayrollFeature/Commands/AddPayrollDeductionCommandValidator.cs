using FluentValidation;

namespace UniversityMS.Application.Features.PayrollFeature.Commands;

/// <summary>
/// Bordro Kesinti Ekleme Command Validator'ı
/// </summary>
public class AddPayrollDeductionCommandValidator : AbstractValidator<AddPayrollDeductionCommand>
{
    public AddPayrollDeductionCommandValidator()
    {
        RuleFor(x => x.PayrollId)
            .NotEmpty().WithMessage("Bordro ID boş olamaz.")
            .NotEqual(Guid.Empty).WithMessage("Bordro ID geçerli olmalıdır.");

        RuleFor(x => x.Type)
            .NotEmpty().WithMessage("Kesinti tipi boş olamaz.")
            .Must(IsValidDeductionType).WithMessage("Geçersiz kesinti tipi.");

        RuleFor(x => x.Description)
            .NotEmpty().WithMessage("Açıklama boş olamaz.")
            .MaximumLength(500).WithMessage("Açıklama en fazla 500 karakter olabilir.");

        RuleFor(x => x.Amount)
            .GreaterThan(0).WithMessage("Kesinti tutarı sıfırdan büyük olmalıdır.")
            .LessThanOrEqualTo(999999999).WithMessage("Kesinti tutarı geçersiz.");

        RuleFor(x => x.Rate)
            .GreaterThanOrEqualTo(0).When(x => x.Rate.HasValue).WithMessage("Oran negatif olamaz.")
            .LessThanOrEqualTo(100).When(x => x.Rate.HasValue).WithMessage("Oran %100'den fazla olamaz.");
    }

    private bool IsValidDeductionType(string type)
    {
        var validTypes = new[] { "IncomeTax", "SGK", "UnemploymentInsurance", "StampDuty", "UnionFee", "Other" };
        return validTypes.Contains(type);
    }
}