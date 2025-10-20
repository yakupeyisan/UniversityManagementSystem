using FluentValidation;

namespace UniversityMS.Application.Features.PayrollFeature.Commands;

/// <summary>
/// Bordro Kalemi Ekleme Command Validator'ı
/// </summary>
public class AddPayrollItemCommandValidator : AbstractValidator<AddPayrollItemCommand>
{
    public AddPayrollItemCommandValidator()
    {
        RuleFor(x => x.PayrollId)
            .NotEmpty().WithMessage("Bordro ID boş olamaz.")
            .NotEqual(Guid.Empty).WithMessage("Bordro ID geçerli olmalıdır.");

        RuleFor(x => x.Type)
            .NotEmpty().WithMessage("Kalem tipi boş olamaz.")
            .Must(IsValidItemType).WithMessage("Geçersiz kalem tipi. Earning veya Deduction olmalıdır.");

        RuleFor(x => x.Category)
            .NotEmpty().WithMessage("Kategori boş olamaz.")
            .MaximumLength(100).WithMessage("Kategori en fazla 100 karakter olabilir.");

        RuleFor(x => x.Description)
            .NotEmpty().WithMessage("Açıklama boş olamaz.")
            .MaximumLength(500).WithMessage("Açıklama en fazla 500 karakter olabilir.");

        RuleFor(x => x.Amount)
            .GreaterThan(0).WithMessage("Tutar sıfırdan büyük olmalıdır.")
            .LessThanOrEqualTo(999999999).WithMessage("Tutar geçersiz.");

        RuleFor(x => x.Quantity)
            .GreaterThan(0).When(x => x.Quantity.HasValue).WithMessage("Miktar sıfırdan büyük olmalıdır.")
            .LessThanOrEqualTo(1000).When(x => x.Quantity.HasValue).WithMessage("Miktar geçersiz.");
    }

    private bool IsValidItemType(string type)
    {
        var validTypes = new[] { "Earning", "Deduction" };
        return validTypes.Contains(type);
    }
}