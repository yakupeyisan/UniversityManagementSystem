using FluentValidation;

namespace UniversityMS.Application.Features.PayrollFeature.Commands;

public class CalculatePayrollCommandValidator : AbstractValidator<CalculatePayrollCommand>
{
    public CalculatePayrollCommandValidator()
    {
        RuleFor(x => x.PayrollId)
            .NotEmpty().WithMessage("Bordro ID gereklidir.")
            .NotEqual(Guid.Empty).WithMessage("Bordro ID boş GUID olamaz.");
    }
}