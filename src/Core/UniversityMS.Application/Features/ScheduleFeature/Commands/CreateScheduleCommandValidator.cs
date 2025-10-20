using FluentValidation;

namespace UniversityMS.Application.Features.ScheduleFeature.Commands;

public class CreateScheduleCommandValidator : AbstractValidator<CreateScheduleCommand>
{
    public CreateScheduleCommandValidator()
    {
        RuleFor(x => x.AcademicYear)
            .NotEmpty().WithMessage("Akademik yıl boş olamaz.")
            .Matches(@"^\d{4}-\d{4}$")
            .WithMessage("Format: 2024-2025");

        RuleFor(x => x.Semester)
            .InclusiveBetween(1, 2)
            .WithMessage("Dönem 1 (Güz) veya 2 (Bahar) olmalıdır.");

        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Program adı gereklidir.")
            .MaximumLength(200).WithMessage("Max 200 karakter.");

        RuleFor(x => x.StartDate)
            .NotEmpty()
            .LessThan(x => x.EndDate)
            .WithMessage("Başlangıç tarihi, bitiş tarihinden önce olmalıdır.");

        RuleFor(x => x.EndDate)
            .NotEmpty();
    }
}