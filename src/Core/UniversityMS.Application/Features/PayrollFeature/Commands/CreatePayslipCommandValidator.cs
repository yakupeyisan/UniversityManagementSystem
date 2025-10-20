using FluentValidation;
using UniversityMS.Application.Features.PayrollFeature.DTOs;

namespace UniversityMS.Application.Features.PayrollFeature.Commands;

/// <summary>
/// Payslip Oluşturma Command Validator'ı (DTOs klasöründe yer alıyor)
/// </summary>
public class CreatePayslipCommandValidator : AbstractValidator<CreatePayslipCommand>
{
    public CreatePayslipCommandValidator()
    {
        RuleFor(x => x.PayrollId)
            .NotEmpty().WithMessage("Bordro ID boş olamaz.")
            .NotEqual(Guid.Empty).WithMessage("Bordro ID geçerli olmalıdır.");
    }
}