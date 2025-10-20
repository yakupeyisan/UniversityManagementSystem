using FluentValidation;

namespace UniversityMS.Application.Features.PayrollFeature.Commands;

/// <summary>
/// Payslip Oluşturma Command Validator'ı
/// </summary>
public class GeneratePayslipCommandValidator : AbstractValidator<GeneratePayslipCommand>
{
    public GeneratePayslipCommandValidator()
    {
        RuleFor(x => x.PayrollId)
            .NotEmpty().WithMessage("Bordro ID boş olamaz.")
            .NotEqual(Guid.Empty).WithMessage("Bordro ID geçerli olmalıdır.");
    }
}