using FluentValidation;

namespace UniversityMS.Application.Features.Attendances.Commands;

public class TakeAttendanceCommandValidator : AbstractValidator<TakeAttendanceCommand>
{
    public TakeAttendanceCommandValidator()
    {
        RuleFor(x => x.CourseId)
            .NotEmpty().WithMessage("Ders ID gereklidir.");

        RuleFor(x => x.InstructorId)
            .NotEmpty().WithMessage("Öğretim görevlisi ID gereklidir.");

        RuleFor(x => x.AttendanceDate)
            .NotEmpty().WithMessage("Yoklama tarihi gereklidir.")
            .LessThanOrEqualTo(DateTime.UtcNow).WithMessage("Yoklama tarihi gelecekte olamaz.");

        RuleFor(x => x.WeekNumber)
            .InclusiveBetween(1, 16).WithMessage("Hafta numarası 1-16 arasında olmalıdır.");

        RuleFor(x => x.Attendances)
            .NotEmpty().WithMessage("En az bir öğrenci yoklaması girilmelidir.");

        RuleForEach(x => x.Attendances).ChildRules(attendance =>
        {
            attendance.RuleFor(x => x.StudentId)
                .NotEmpty().WithMessage("Öğrenci ID gereklidir.");

            attendance.RuleFor(x => x.CourseRegistrationId)
                .NotEmpty().WithMessage("Ders kayıt ID gereklidir.");
        });
    }
}