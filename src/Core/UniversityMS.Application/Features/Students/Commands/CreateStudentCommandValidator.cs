using FluentValidation;

namespace UniversityMS.Application.Features.Students.Commands;

public class CreateStudentCommandValidator : AbstractValidator<CreateStudentCommand>
{
    public CreateStudentCommandValidator()
    {
        RuleFor(x => x.FirstName)
            .NotEmpty().WithMessage("Ad boş olamaz.")
            .MaximumLength(100).WithMessage("Ad en fazla 100 karakter olabilir.");

        RuleFor(x => x.LastName)
            .NotEmpty().WithMessage("Soyad boş olamaz.")
            .MaximumLength(100).WithMessage("Soyad en fazla 100 karakter olabilir.");

        RuleFor(x => x.NationalId)
            .NotEmpty().WithMessage("TC Kimlik No boş olamaz.")
            .Length(11).WithMessage("TC Kimlik No 11 haneli olmalıdır.")
            .Matches("^[0-9]+$").WithMessage("TC Kimlik No sadece rakamlardan oluşmalıdır.");

        RuleFor(x => x.BirthDate)
            .NotEmpty().WithMessage("Doğum tarihi boş olamaz.")
            .LessThan(DateTime.Today.AddYears(-15)).WithMessage("Öğrenci en az 15 yaşında olmalıdır.");

        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("E-posta adresi boş olamaz.")
            .EmailAddress().WithMessage("Geçerli bir e-posta adresi giriniz.");

        RuleFor(x => x.PhoneNumber)
            .NotEmpty().WithMessage("Telefon numarası boş olamaz.")
            .Matches("^[0-9]{10,11}$").WithMessage("Geçerli bir telefon numarası giriniz.");

        RuleFor(x => x.StudentNumber)
            .NotEmpty().WithMessage("Öğrenci numarası boş olamaz.")
            .MaximumLength(20).WithMessage("Öğrenci numarası en fazla 20 karakter olabilir.");

        RuleFor(x => x.DepartmentId)
            .NotEmpty().WithMessage("Bölüm seçilmelidir.");
    }
}