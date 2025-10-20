using FluentValidation;

namespace UniversityMS.Application.Features.DepartmentFeature.Commands;

public class AssignDepartmentHeadCommandValidator : AbstractValidator<AssignDepartmentHeadCommand>
{
    public AssignDepartmentHeadCommandValidator()
    {
        RuleFor(x => x.DepartmentId)
            .NotEmpty().WithMessage("Bölüm ID gereklidir.")
            .NotEqual(Guid.Empty).WithMessage("Bölüm ID boş GUID olamaz.");

        RuleFor(x => x.FacultyId)
            .NotEmpty().WithMessage("Fakülte ID gereklidir.")
            .NotEqual(Guid.Empty).WithMessage("Fakülte ID boş GUID olamaz.");

        RuleFor(x => x)
            .Must(x => x.DepartmentId != x.FacultyId)
            .WithMessage("Bölüm ve Fakülte ID'leri aynı olamaz.");
    }
}