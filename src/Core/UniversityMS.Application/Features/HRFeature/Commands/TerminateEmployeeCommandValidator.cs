using FluentValidation;

namespace UniversityMS.Application.Features.HRFeature.Commands;

public class TerminateEmployeeCommandValidator : AbstractValidator<TerminateEmployeeCommand>
{
    public TerminateEmployeeCommandValidator()
    {
        RuleFor(x => x.EmployeeId)
            .NotEmpty().WithMessage("Çalışan ID gereklidir.")
            .NotEqual(Guid.Empty).WithMessage("Çalışan ID boş GUID olamaz.");

        RuleFor(x => x.TerminationDate)
            .NotEmpty().WithMessage("Sonlandırma tarihi gereklidir.")
            .GreaterThanOrEqualTo(DateTime.UtcNow.Date)
            .WithMessage("Sonlandırma tarihi geçmişte olamaz.");
    }
}