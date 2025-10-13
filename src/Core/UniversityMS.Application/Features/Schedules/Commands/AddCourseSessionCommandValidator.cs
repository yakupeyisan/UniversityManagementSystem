using FluentValidation;

namespace UniversityMS.Application.Features.Schedules.Commands;

public class AddCourseSessionCommandValidator : AbstractValidator<AddCourseSessionCommand>
{
    public AddCourseSessionCommandValidator()
    {
        RuleFor(x => x.ScheduleId)
            .NotEmpty().WithMessage("Program ID gereklidir.");

        RuleFor(x => x.CourseId)
            .NotEmpty().WithMessage("Ders ID gereklidir.");

        RuleFor(x => x.ClassroomId)
            .NotEmpty().WithMessage("Derslik ID gereklidir.");

        RuleFor(x => x.StartTime)
            .NotEmpty().WithMessage("Başlangıç saati gereklidir.")
            .Matches(@"^([0-1]?[0-9]|2[0-3]):[0-5][0-9]$")
            .WithMessage("Saat formatı HH:mm olmalıdır (örn: 09:00)");

        RuleFor(x => x.EndTime)
            .NotEmpty().WithMessage("Bitiş saati gereklidir.")
            .Matches(@"^([0-1]?[0-9]|2[0-3]):[0-5][0-9]$")
            .WithMessage("Saat formatı HH:mm olmalıdır (örn: 10:50)");

        RuleFor(x => x.DayOfWeek)
            .IsInEnum().WithMessage("Geçerli bir gün seçilmelidir.");

        RuleFor(x => x.SessionType)
            .IsInEnum().WithMessage("Geçerli bir oturum tipi seçilmelidir.");

        RuleFor(x => x.Notes)
            .MaximumLength(500).WithMessage("Notlar en fazla 500 karakter olabilir.");
    }
}