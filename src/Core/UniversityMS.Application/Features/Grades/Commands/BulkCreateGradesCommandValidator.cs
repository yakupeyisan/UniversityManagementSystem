using FluentValidation;

namespace UniversityMS.Application.Features.Grades.Commands;

public class BulkCreateGradesCommandValidator : AbstractValidator<BulkCreateGradesCommand>
{
    public BulkCreateGradesCommandValidator()
    {
        RuleFor(x => x.Grades)
            .NotEmpty().WithMessage("En az bir not girişi yapılmalıdır.");

        RuleForEach(x => x.Grades).ChildRules(grade =>
        {
            grade.RuleFor(x => x.CourseRegistrationId)
                .NotEmpty().WithMessage("Ders kayıt ID gereklidir.");

            grade.RuleFor(x => x.StudentId)
                .NotEmpty().WithMessage("Öğrenci ID gereklidir.");

            grade.RuleFor(x => x.CourseId)
                .NotEmpty().WithMessage("Ders ID gereklidir.");

            grade.RuleFor(x => x.NumericScore)
                .InclusiveBetween(0, 100).WithMessage("Not 0-100 arasında olmalıdır.");

            grade.RuleFor(x => x.Weight)
                .InclusiveBetween(0, 1).WithMessage("Ağırlık 0-1 arasında olmalıdır.");
        });
    }
}