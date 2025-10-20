using FluentValidation;

namespace UniversityMS.Application.Features.PayrollFeature.Commands;

/// <summary>
/// Payslip Email Gönderme Command Validator'ı
/// </summary>
public class SendPayslipEmailCommandValidator : AbstractValidator<SendPayslipEmailCommand>
{
    public SendPayslipEmailCommandValidator()
    {
        RuleFor(x => x.PayslipId)
            .NotEmpty().WithMessage("Payslip ID boş olamaz.")
            .NotEqual(Guid.Empty).WithMessage("Payslip ID geçerli olmalıdır.");

        RuleFor(x => x.OverrideEmail)
            .EmailAddress().When(x => !string.IsNullOrEmpty(x.OverrideEmail))
            .WithMessage("Geçerli bir email adresi giriniz.")
            .MaximumLength(256).When(x => !string.IsNullOrEmpty(x.OverrideEmail))
            .WithMessage("Email adresi en fazla 256 karakter olabilir.");

        RuleFor(x => x.EmailTemplate)
            .MaximumLength(100).When(x => !string.IsNullOrEmpty(x.EmailTemplate))
            .WithMessage("Email şablonu adı en fazla 100 karakter olabilir.")
            .Matches("^[a-zA-Z0-9_-]*$").When(x => !string.IsNullOrEmpty(x.EmailTemplate))
            .WithMessage("Email şablonu adı sadece harfler, rakamlar, tire ve alt çizgi içerebilir.");
    }
}