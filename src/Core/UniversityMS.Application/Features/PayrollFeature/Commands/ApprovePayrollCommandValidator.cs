using FluentValidation;

namespace UniversityMS.Application.Features.PayrollFeature.Commands;

public class ApprovePayrollCommandValidator : AbstractValidator<ApprovePayrollCommand>
{
    public ApprovePayrollCommandValidator()
    {
        RuleFor(x => x.PayrollId)
            .NotEmpty().WithMessage("Bordro ID gereklidir.")
            .NotEqual(Guid.Empty).WithMessage("Bordro ID boş GUID olamaz.");
    }
}