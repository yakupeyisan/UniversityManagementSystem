using FluentValidation;

namespace UniversityMS.Application.Features.PayrollFeature.Commands;

public class CreatePayrollCommandValidator : AbstractValidator<CreatePayrollCommand>
{
    public CreatePayrollCommandValidator()
    {
        RuleFor(x => x.EmployeeId)
            .NotEqual(Guid.Empty).WithMessage("Çalışan seçilmelidir");

        RuleFor(x => x.Month)
            .GreaterThanOrEqualTo(1).WithMessage("Ay 1'den başlamalıdır")
            .LessThanOrEqualTo(12).WithMessage("Ay 12'den fazla olamaz");

        RuleFor(x => x.Year)
            .GreaterThanOrEqualTo(2020).WithMessage("Yıl 2020'den önceki olamaz")
            .LessThanOrEqualTo(DateTime.Now.Year).WithMessage("Gelecek yıl seçilemez");

        RuleFor(x => x.BaseSalary)
            .GreaterThan(0).WithMessage("Temel maaş 0'dan büyük olmalıdır");

        RuleFor(x => x.Bonus)
            .GreaterThanOrEqualTo(0).When(x => x.Bonus.HasValue).WithMessage("Bonus negatif olamaz");
    }
}