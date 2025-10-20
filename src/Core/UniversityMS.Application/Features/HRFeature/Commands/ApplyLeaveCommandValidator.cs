using FluentValidation;
using UniversityMS.Application.Features.StaffFeature.Commands;

namespace UniversityMS.Application.Features.HRFeature.Commands;

public class ApplyLeaveCommandValidator : AbstractValidator<ApplyLeaveCommand>
{
    public ApplyLeaveCommandValidator()
    {
        RuleFor(x => x.EmployeeId)
            .NotEqual(Guid.Empty).WithMessage("Geçerli bir çalışan seçilmelidir.");

        RuleFor(x => x.StartDate)
            .GreaterThanOrEqualTo(DateTime.UtcNow.Date)
            .WithMessage("İzin başlangıç tarihi bugünden itibaren olmalıdır.");

        RuleFor(x => x.EndDate)
            .GreaterThanOrEqualTo(x => x.StartDate)
            .WithMessage("İzin bitiş tarihi başlangıç tarihinden sonra olmalıdır.");

        RuleFor(x => x.Reason)
            .NotEmpty().WithMessage("İzin sebebi belirtilmelidir.")
            .Length(5, 500).WithMessage("İzin sebebi 5-500 karakter arasında olmalıdır.");

        RuleFor(x => x.LeaveTypeId)
            .InclusiveBetween(1, 5).WithMessage("Geçerli bir izin türü seçilmelidir.");
    }
}