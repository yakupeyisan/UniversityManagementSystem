using FluentValidation;

namespace UniversityMS.Application.Features.GradeFeature.Commands;

public class SubmitGradeCommandValidator : AbstractValidator<SubmitGradeCommand>
{
    public SubmitGradeCommandValidator()
    {
        RuleFor(x => x.CourseRegistrationId)
            .NotEmpty().WithMessage("Ders kayıt ID gereklidir.");

        RuleFor(x => x.StudentId)
            .NotEmpty().WithMessage("Öğrenci ID gereklidir.");

        RuleFor(x => x.CourseId)
            .NotEmpty().WithMessage("Ders ID gereklidir.");

        RuleFor(x => x.NumericScore)
            .InclusiveBetween(0, 100).WithMessage("Not 0-100 arasında olmalıdır.");

        RuleFor(x => x.Weight)
            .InclusiveBetween(0, 1).WithMessage("Ağırlık 0-1 arasında olmalıdır.");
    }
}