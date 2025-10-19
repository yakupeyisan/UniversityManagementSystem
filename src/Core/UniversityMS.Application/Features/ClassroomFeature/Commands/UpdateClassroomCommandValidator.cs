using FluentValidation;

namespace UniversityMS.Application.Features.ClassroomFeature.Commands;

public class UpdateClassroomCommandValidator : AbstractValidator<UpdateClassroomCommand>
{
    public UpdateClassroomCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Derslik ID gereklidir.");

        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Derslik adı boş olamaz.")
            .MaximumLength(200).WithMessage("Derslik adı en fazla 200 karakter olabilir.");

        RuleFor(x => x.Capacity)
            .GreaterThan(0).WithMessage("Kapasite 0'dan büyük olmalıdır.");

        When(x => x.HasComputers, () =>
        {
            RuleFor(x => x.ComputerCount)
                .GreaterThan(0).WithMessage("Bilgisayar sayısı belirtilmelidir.");
        });
    }
}