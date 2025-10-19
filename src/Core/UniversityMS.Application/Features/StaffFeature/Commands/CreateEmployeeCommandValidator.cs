using FluentValidation;

namespace UniversityMS.Application.Features.StaffFeature.Commands;

public class CreateEmployeeCommandValidator : AbstractValidator<CreateEmployeeCommand>
{
    public CreateEmployeeCommandValidator()
    {
        RuleFor(x => x.FirstName)
            .NotEmpty().WithMessage("Ad boş olamaz.")
            .MaximumLength(100).WithMessage("Ad en fazla 100 karakter olabilir.");

        RuleFor(x => x.LastName)
            .NotEmpty().WithMessage("Soyadı boş olamaz.")
            .MaximumLength(100).WithMessage("Soyadı en fazla 100 karakter olabilir.");

        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email boş olamaz.")
            .EmailAddress().WithMessage("Geçerli bir email giriniz.");

        RuleFor(x => x.PhoneNumber)
            .NotEmpty().WithMessage("Telefon numarası boş olamaz.")
            .Length(11).WithMessage("Telefon numarası 11 haneli olmalıdır.");

        RuleFor(x => x.Position)
            .NotEmpty().WithMessage("Pozisyon boş olamaz.")
            .MaximumLength(100).WithMessage("Pozisyon en fazla 100 karakter olabilir.");

        RuleFor(x => x.DepartmentId)
            .NotEmpty().WithMessage("Bölüm seçimi zorunludur.");

        RuleFor(x => x.HireDate)
            .LessThanOrEqualTo(DateTime.Now).WithMessage("İşe giriş tarihi gelecekte olamaz.");
    }
}