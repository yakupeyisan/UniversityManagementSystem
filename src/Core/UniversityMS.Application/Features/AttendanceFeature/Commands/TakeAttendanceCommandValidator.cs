using FluentValidation;

namespace UniversityMS.Application.Features.AttendanceFeature.Commands;

public class TakeAttendanceCommandValidator : AbstractValidator<TakeAttendanceCommand>
{
    public TakeAttendanceCommandValidator()
    {
        RuleFor(x => x.CourseRegistrationId)
            .NotEmpty().WithMessage("Ders kaydı ID boş olamaz.");

        RuleFor(x => x.WeekNumber)
            .GreaterThan(0).WithMessage("Hafta numarası 0'dan büyük olmalıdır.");
    }
}
