using FluentValidation;

namespace UniversityMS.Application.Features.ClassroomFeature.Commands;

public class CreateClassroomCommandValidator : AbstractValidator<CreateClassroomCommand>
{
    public CreateClassroomCommandValidator()
    {
        RuleFor(x => x.Code)
            .NotEmpty().WithMessage("Derslik kodu boş olamaz.")
            .MaximumLength(20).WithMessage("Derslik kodu en fazla 20 karakter olabilir.");

        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Derslik adı boş olamaz.")
            .MaximumLength(200).WithMessage("Derslik adı en fazla 200 karakter olabilir.");

        RuleFor(x => x.Capacity)
            .GreaterThan(0).WithMessage("Kapasite 0'dan büyük olmalıdır.");

        RuleFor(x => x.Type)
            .IsInEnum().WithMessage("Geçerli bir derslik tipi seçilmelidir.");

        RuleFor(x => x.Building)
            .MaximumLength(100).WithMessage("Bina adı en fazla 100 karakter olabilir.");

        RuleFor(x => x.Description)
            .MaximumLength(1000).WithMessage("Açıklama en fazla 1000 karakter olabilir.");

        When(x => x.HasComputers, () =>
        {
            RuleFor(x => x.ComputerCount)
                .GreaterThan(0).WithMessage("Bilgisayar sayısı belirtilmelidir.");
        });
    }
}