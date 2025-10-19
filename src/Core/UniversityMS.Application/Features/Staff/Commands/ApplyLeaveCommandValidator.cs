using FluentValidation;

namespace UniversityMS.Application.Features.Staff.Commands;

public class ApplyLeaveCommandValidator : AbstractValidator<ApplyLeaveCommand>
{
    public ApplyLeaveCommandValidator()
    {
        RuleFor(x => x.EmployeeId)
            .NotEmpty().WithMessage("Çalışan ID boş olamaz");

        RuleFor(x => x.StartDate)
            .NotEmpty().WithMessage("Başlangıç tarihi boş olamaz")
            .GreaterThanOrEqualTo(DateTime.UtcNow.Date).WithMessage("Başlangıç tarihi bugüne eşit ya da sonra olmalı");

        RuleFor(x => x.EndDate)
            .NotEmpty().WithMessage("Bitiş tarihi boş olamaz")
            .GreaterThanOrEqualTo(x => x.StartDate).WithMessage("Bitiş tarihi başlangıç tarihine eşit ya da sonra olmalı");

        RuleFor(x => x.Reason)
            .NotEmpty().WithMessage("İzin nedeni boş olamaz")
            .MaximumLength(500).WithMessage("İzin nedeni 500 karakteri geçemez");
    }
}