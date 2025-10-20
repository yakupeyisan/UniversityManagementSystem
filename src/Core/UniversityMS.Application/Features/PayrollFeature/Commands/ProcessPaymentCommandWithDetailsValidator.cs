using FluentValidation;

namespace UniversityMS.Application.Features.PayrollFeature.Commands;

public class ProcessPaymentCommandWithDetailsValidator : AbstractValidator<ProcessPaymentCommandWithDetails>
{
    public ProcessPaymentCommandWithDetailsValidator()
    {
        RuleFor(x => x.PayrollId)
            .NotEmpty().WithMessage("Bordro ID gereklidir.")
            .NotEqual(Guid.Empty).WithMessage("Bordro ID boş GUID olamaz.");

        RuleFor(x => x.BankName)
            .NotEmpty().WithMessage("Banka adı gereklidir.")
            .MaximumLength(100).WithMessage("Banka adı en fazla 100 karakter olabilir.");

        RuleFor(x => x.TransactionReference)
            .MaximumLength(100).When(x => !string.IsNullOrEmpty(x.TransactionReference))
            .WithMessage("İşlem referansı en fazla 100 karakter olabilir.");
    }
}