using FluentValidation;

namespace UniversityMS.Application.Features.Staff.Commands;

public class CreateEmployeeCommandValidator : AbstractValidator<CreateEmployeeCommand>
{
    public CreateEmployeeCommandValidator()
    {
        RuleFor(x => x.PersonId)
            .NotEmpty().WithMessage("Kişi ID boş olamaz");

        RuleFor(x => x.EmployeeNumber)
            .NotEmpty().WithMessage("Çalışan numarası boş olamaz")
            .Length(3, 20).WithMessage("Çalışan numarası 3-20 karakter arasında olmalı");

        RuleFor(x => x.JobTitle)
            .NotEmpty().WithMessage("İş unvanı boş olamaz")
            .MaximumLength(100).WithMessage("İş unvanı 100 karakteri geçemez");

        RuleFor(x => x.HireDate)
            .NotEmpty().WithMessage("İşe alım tarihi boş olamaz")
            .LessThanOrEqualTo(DateTime.UtcNow).WithMessage("İşe alım tarihi bugünden sonra olamaz");

        RuleFor(x => x.BaseSalary)
            .GreaterThan(0).WithMessage("Temel maaş 0'dan büyük olmalı")
            .LessThan(1000000).WithMessage("Temel maaş 1.000.000'den küçük olmalı");

        RuleFor(x => x.WorkingHoursPerWeek)
            .GreaterThan(0).WithMessage("Haftalık çalışma saati 0'dan büyük olmalı")
            .LessThanOrEqualTo(60).WithMessage("Haftalık çalışma saati 60'dan fazla olamaz");
    }
}