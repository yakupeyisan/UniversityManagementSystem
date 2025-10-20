using FluentValidation;

namespace UniversityMS.Application.Features.ScheduleFeature.Commands;

public class UpdateScheduleCommandValidator : AbstractValidator<UpdateScheduleCommand>
{
    public UpdateScheduleCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Program ID gereklidir.");

        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Program adı boş olamaz.")
            .MaximumLength(200).WithMessage("Program adı en fazla 200 karakter olabilir.");

        RuleFor(x => x.Description)
            .MaximumLength(1000).WithMessage("Açıklama en fazla 1000 karakter olabilir.");

        RuleFor(x => x.StartDate)
            .NotEmpty().WithMessage("Başlangıç tarihi boş olamaz.")
            .LessThan(x => x.EndDate)
            .WithMessage("Başlangıç tarihi bitiş tarihinden önce olmalıdır.");

        RuleFor(x => x.EndDate)
            .NotEmpty().WithMessage("Bitiş tarihi boş olamaz.");

        RuleFor(x => x.Status)
            .IsInEnum().WithMessage("Geçerli bir statü seçilmelidir.");
    }
}