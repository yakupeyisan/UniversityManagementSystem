using FluentValidation;

namespace UniversityMS.Application.Features.HR.Commands;

public class CreateEmployeeCommandValidator : AbstractValidator<CreateEmployeeCommand>
{
    public CreateEmployeeCommandValidator()
    {
        RuleFor(x => x.PersonId)
            .NotEqual(Guid.Empty).WithMessage("Kişi seçilmelidir");

        RuleFor(x => x.EmployeeNumber)
            .NotEmpty().WithMessage("Çalışan numarası boş olamaz")
            .Length(5, 20).WithMessage("Çalışan numarası 5-20 karakter arasında olmalıdır");

        RuleFor(x => x.JobTitle)
            .NotEmpty().WithMessage("İş ünvanı boş olamaz")
            .MaximumLength(100).WithMessage("İş ünvanı 100 karakterden uzun olamaz");

        RuleFor(x => x.HireDate)
            .LessThanOrEqualTo(DateTime.Now).WithMessage("İşe alım tarihi gelecekte olamaz");

        RuleFor(x => x.BaseSalary)
            .GreaterThan(0).WithMessage("Maaş 0'dan büyük olmalıdır");

        RuleFor(x => x.WeeklyHours)
            .GreaterThan(0).WithMessage("Haftalık çalışma saati 0'dan büyük olmalıdır")
            .LessThanOrEqualTo(60).WithMessage("Haftalık çalışma saati 60 saati geçemez");

        RuleFor(x => x.EndTime)
            .GreaterThan(x => x.StartTime).WithMessage("Bitiş saati başlangıç saatinden sonra olmalıdır");
    }
}