using FluentValidation;

namespace UniversityMS.Application.Features.Attendances.Commands;

public class QRCheckInCommandValidator : AbstractValidator<QRCheckInCommand>
{
    public QRCheckInCommandValidator()
    {
        RuleFor(x => x.StudentId)
            .NotEmpty().WithMessage("Öğrenci ID gereklidir.");

        RuleFor(x => x.CourseId)
            .NotEmpty().WithMessage("Ders ID gereklidir.");

        RuleFor(x => x.CourseRegistrationId)
            .NotEmpty().WithMessage("Ders kayıt ID gereklidir.");

        RuleFor(x => x.QRCode)
            .NotEmpty().WithMessage("QR kodu gereklidir.");

        RuleFor(x => x.WeekNumber)
            .InclusiveBetween(1, 16).WithMessage("Hafta numarası 1-16 arasında olmalıdır.");
    }
}