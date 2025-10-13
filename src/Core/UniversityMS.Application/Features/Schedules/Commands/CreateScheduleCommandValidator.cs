using FluentValidation;

namespace UniversityMS.Application.Features.Schedules.Commands;

public class CreateScheduleCommandValidator : AbstractValidator<CreateScheduleCommand>
{
    public CreateScheduleCommandValidator()
    {
        RuleFor(x => x.AcademicYear)
            .NotEmpty().WithMessage("Akademik yıl boş olamaz.")
            .Matches(@"^\d{4}-\d{4}$").WithMessage("Akademik yıl formatı 2024-2025 şeklinde olmalıdır.");

        RuleFor(x => x.Semester)
            .InclusiveBetween(1, 2).WithMessage("Dönem 1 (Güz) veya 2 (Bahar) olmalıdır.");

        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Program adı boş olamaz.")
            .MaximumLength(200).WithMessage("Program adı en fazla 200 karakter olabilir.");

        RuleFor(x => x.Description)
            .MaximumLength(1000).WithMessage("Açıklama en fazla 1000 karakter olabilir.");

        RuleFor(x => x.StartDate)
            .NotEmpty().WithMessage("Başlangıç tarihi boş olamaz.")
            .LessThan(x => x.EndDate).WithMessage("Başlangıç tarihi bitiş tarihinden önce olmalıdır.");

        RuleFor(x => x.EndDate)
            .NotEmpty().WithMessage("Bitiş tarihi boş olamaz.");
    }
}