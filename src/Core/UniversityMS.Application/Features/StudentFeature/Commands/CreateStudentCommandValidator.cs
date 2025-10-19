using FluentValidation;

namespace UniversityMS.Application.Features.StudentFeature.Commands;

public class CreateStudentCommandValidator : AbstractValidator<CreateStudentCommand>
{
    public CreateStudentCommandValidator()
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
            .Matches(@"^\d{10,}$").WithMessage("Geçerli bir telefon numarası giriniz.");

        RuleFor(x => x.NationalId)
            .NotEmpty().WithMessage("TC Kimlik No boş olamaz.")
            .Length(11).WithMessage("TC Kimlik No 11 haneli olmalıdır.");

        RuleFor(x => x.StudentNumber)
            .NotEmpty().WithMessage("Öğrenci numarası boş olamaz.");

        RuleFor(x => x.BirthDate)
            .LessThan(DateTime.Today).WithMessage("Doğum tarihi günümüzden önce olmalıdır.");

        RuleFor(x => x.DepartmentId)
            .NotEmpty().WithMessage("Bölüm seçimi zorunludur.");
    }
}
