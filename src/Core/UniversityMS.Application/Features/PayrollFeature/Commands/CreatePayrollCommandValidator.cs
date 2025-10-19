using FluentValidation;

namespace UniversityMS.Application.Features.PayrollFeature.Commands;

public class CreatePayrollCommandValidator : AbstractValidator<CreatePayrollCommand>
{
    public CreatePayrollCommandValidator()
    {
        RuleFor(x => x.EmployeeId)
            .NotEmpty().WithMessage("Çalışan ID boş olamaz");

        RuleFor(x => x.Month)
            .GreaterThanOrEqualTo(1).WithMessage("Ay 1'den az olamaz")
            .LessThanOrEqualTo(12).WithMessage("Ay 12'den fazla olamaz");

        RuleFor(x => x.Year)
            .GreaterThanOrEqualTo(2020).WithMessage("Yıl 2020'den az olamaz")
            .LessThanOrEqualTo(DateTime.UtcNow.Year + 1).WithMessage("Gelecek yıl bordrosu oluşturulamaz");

        RuleFor(x => x.BaseSalary)
            .GreaterThan(0).WithMessage("Temel maaş 0'dan büyük olmalı");

        RuleFor(x => x.WorkingDays)
            .GreaterThan(0).WithMessage("Çalışma günü 0'dan büyük olmalı")
            .LessThanOrEqualTo(31).WithMessage("Çalışma günü 31'den fazla olamaz");
    }
}
