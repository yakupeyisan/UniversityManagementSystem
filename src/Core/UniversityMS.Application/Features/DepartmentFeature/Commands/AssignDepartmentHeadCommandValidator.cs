using FluentValidation;

namespace UniversityMS.Application.Features.DepartmentFeature.Commands;

public class AssignDepartmentHeadCommandValidator : AbstractValidator<AssignDepartmentHeadCommand>
{
    public AssignDepartmentHeadCommandValidator()
    {
        RuleFor(x => x.DepartmentId)
            .NotEmpty().WithMessage("Bölüm ID gereklidir.");

        RuleFor(x => x.FacultyId)
            .NotEmpty().WithMessage("Öğretim üyesi ID gereklidir.");
    }
}