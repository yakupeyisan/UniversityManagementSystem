using FluentValidation;

namespace UniversityMS.Application.Features.StudentFeature.Commands;

public class UpdateStudentCommandValidator : AbstractValidator<UpdateStudentCommand>
{
    public UpdateStudentCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Öğrenci ID boş olamaz.");

        RuleFor(x => x.FirstName)
            .NotEmpty().WithMessage("Ad boş olamaz.")
            .MaximumLength(100).WithMessage("Ad en fazla 100 karakter olabilir.");

        RuleFor(x => x.LastName)
            .NotEmpty().WithMessage("Soyadı boş olamaz.")
            .MaximumLength(100).WithMessage("Soyadı en fazla 100 karakter olabilir.");

        RuleFor(x => x.PhoneNumber)
            .NotEmpty().WithMessage("Telefon numarası boş olamaz.")
            .Matches(@"^\d{10,}$").WithMessage("Geçerli bir telefon numarası giriniz.");
    }
}