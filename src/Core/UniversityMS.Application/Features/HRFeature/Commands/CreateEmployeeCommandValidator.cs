using FluentValidation;

namespace UniversityMS.Application.Features.HRFeature.Commands;

/// <summary>
/// Çalışan oluşturma command'ı validator'ı
/// Girdi validasyonlarını yapar
/// </summary>
public class CreateEmployeeCommandValidator : AbstractValidator<CreateEmployeeCommand>
{
    public CreateEmployeeCommandValidator()
    {
        RuleFor(x => x.EmployeeNumber)
            .NotEmpty().WithMessage("Çalışan numarası boş olamaz.")
            .Length(1, 50).WithMessage("Çalışan numarası 1-50 karakter olmalıdır.");

        RuleFor(x => x.PersonId)
            .NotEmpty().WithMessage("Kişi ID'si boş olamaz.")
            .NotEqual(Guid.Empty).WithMessage("Geçerli bir kişi ID'si giriniz.");

        RuleFor(x => x.JobTitle)
            .NotEmpty().WithMessage("İş ünvanı boş olamaz.")
            .Length(1, 100).WithMessage("İş ünvanı 1-100 karakter olmalıdır.");

        RuleFor(x => x.HireDate)
            .NotEmpty().WithMessage("İşe alım tarihi boş olamaz.")
            .LessThanOrEqualTo(DateTime.UtcNow).WithMessage("İşe alım tarihi gelecekte olamaz.");

        RuleFor(x => x.BaseSalary)
            .GreaterThan(0).WithMessage("Temel maaş 0'dan büyük olmalıdır.")
            .LessThanOrEqualTo(999999999).WithMessage("Temel maaş geçersiz.");

        RuleFor(x => x.StartTime)
            .NotEmpty().WithMessage("Başlangıç saati boş olamaz.");

        RuleFor(x => x.EndTime)
            .NotEmpty().WithMessage("Bitiş saati boş olamaz.")
            .GreaterThan(x => x.StartTime).WithMessage("Bitiş saati başlangıç saatinden sonra olmalıdır.");

        RuleFor(x => x.WeeklyHours)
            .GreaterThan(0).WithMessage("Haftalık çalışma saati 0'dan büyük olmalıdır.")
            .LessThanOrEqualTo(168).WithMessage("Haftalık çalışma saati 168'den fazla olamaz.");

        RuleFor(x => x.DepartmentId)
            .Must(x => x == null || x != Guid.Empty)
            .WithMessage("Geçerli bir departman ID'si giriniz.");

        RuleFor(x => x.Notes)
            .MaximumLength(500).WithMessage("Notlar 500 karakterden fazla olamaz.");
    }
}
