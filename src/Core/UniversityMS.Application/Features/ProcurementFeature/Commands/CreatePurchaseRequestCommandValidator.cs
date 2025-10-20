using FluentValidation;

namespace UniversityMS.Application.Features.ProcurementFeature.Commands;

public class CreatePurchaseRequestCommandValidator : AbstractValidator<CreatePurchaseRequestCommand>
{
    public CreatePurchaseRequestCommandValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty().WithMessage("Başlık gereklidir.")
            .MaximumLength(200).WithMessage("Max 200 karakter.");

        RuleFor(x => x.Description)
            .NotEmpty().WithMessage("Açıklama gereklidir.")
            .MaximumLength(1000).WithMessage("Max 1000 karakter.");

        RuleFor(x => x.RequiredDate)
            .GreaterThan(DateTime.UtcNow).WithMessage("Gerekli tarih gelecek bir tarih olmalıdır.");

        RuleFor(x => x.Priority)
            .Must(p => new[] { "Low", "Medium", "High" }.Contains(p))
            .WithMessage("Geçersiz öncelik: Low, Medium veya High olmalıdır.");
    }
}