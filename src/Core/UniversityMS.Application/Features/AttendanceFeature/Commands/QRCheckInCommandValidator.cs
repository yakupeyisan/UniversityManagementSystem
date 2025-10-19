using FluentValidation;

namespace UniversityMS.Application.Features.AttendanceFeature.Commands;

public class QRCheckInCommandValidator : AbstractValidator<QRCheckInCommand>
{
    public QRCheckInCommandValidator()
    {
        RuleFor(x => x.StudentId)
            .NotEmpty().WithMessage("Öğrenci ID boş olamaz.");

        RuleFor(x => x.CourseId)
            .NotEmpty().WithMessage("Ders ID boş olamaz.");

        RuleFor(x => x.CourseRegistrationId)
            .NotEmpty().WithMessage("Ders kaydı ID boş olamaz.");

        RuleFor(x => x.QRCode)
            .NotEmpty().WithMessage("QR kodu boş olamaz.")
            .MinimumLength(10).WithMessage("QR kodu geçerli bir format olmalıdır.");

        RuleFor(x => x.WeekNumber)
            .GreaterThan(0).WithMessage("Hafta numarası 0'dan büyük olmalıdır.")
            .LessThanOrEqualTo(16).WithMessage("Hafta numarası 16'dan büyük olamaz.");
    }
}