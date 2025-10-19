using FluentValidation;

namespace UniversityMS.Application.Features.AttendanceFeature.Commands;

public class TakeAttendanceCommandValidator : AbstractValidator<TakeAttendanceCommand>
{
    public TakeAttendanceCommandValidator()
    {
        RuleFor(x => x.CourseId)
            .NotEmpty().WithMessage("Ders ID boş olamaz.");

        RuleFor(x => x.InstructorId)
            .NotEmpty().WithMessage("Öğretim üyesi ID boş olamaz.");

        RuleFor(x => x.AttendanceDate)
            .NotEmpty().WithMessage("Devam tarihi boş olamaz.")
            .LessThanOrEqualTo(DateTime.UtcNow).WithMessage("Devam tarihi günümüzden ileri olamaz.");

        RuleFor(x => x.WeekNumber)
            .GreaterThan(0).WithMessage("Hafta numarası 0'dan büyük olmalıdır.")
            .LessThanOrEqualTo(16).WithMessage("Hafta numarası 16'dan büyük olamaz.");

        RuleFor(x => x.Attendances)
            .NotEmpty().WithMessage("En az bir devam kaydı olmalıdır.")
            .Must(a => a.All(att => !Guid.Empty.Equals(att.StudentId)))
            .WithMessage("Tüm öğrenci ID'leri geçerli olmalıdır.");
    }
}
