using FluentValidation;

namespace UniversityMS.Application.Features.PayrollFeature.Commands;

public class ProcessPaymentCommandValidator : AbstractValidator<ProcessPaymentCommand>
{
    public ProcessPaymentCommandValidator()
    {
        RuleFor(x => x.PayrollId)
            .NotEmpty().WithMessage("Bordro ID gereklidir.")
            .NotEqual(Guid.Empty).WithMessage("Bordro ID boş GUID olamaz.");
    }
}