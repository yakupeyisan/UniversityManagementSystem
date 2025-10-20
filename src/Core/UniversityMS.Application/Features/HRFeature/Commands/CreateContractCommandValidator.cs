using FluentValidation;

namespace UniversityMS.Application.Features.HRFeature.Commands;

public class CreateContractCommandValidator : AbstractValidator<CreateContractCommand>
{
    public CreateContractCommandValidator()
    {
        RuleFor(x => x.EmployeeId)
            .NotEqual(Guid.Empty).WithMessage("Geçerli bir çalışan seçilmelidir.");

        RuleFor(x => x.ContractNumber)
            .NotEmpty().WithMessage("Sözleşme numarası boş olamaz.")
            .Length(3, 30).WithMessage("Sözleşme numarası 3-30 karakter arasında olmalıdır.");

        RuleFor(x => x.StartDate)
            .LessThanOrEqualTo(DateTime.UtcNow.Date).WithMessage("Başlangıç tarihi geçmiş olmalıdır.");

        RuleFor(x => x.EndDate)
            .GreaterThan(x => x.StartDate).When(x => x.EndDate.HasValue)
            .WithMessage("Bitiş tarihi başlangıç tarihinden sonra olmalıdır.");

        RuleFor(x => x.BaseSalary)
            .GreaterThan(0).WithMessage("Maaş sıfırdan büyük olmalıdır.");

        RuleFor(x => x.ContractType)
            .Must(ct => ct == "Permanent" || ct == "FixedTerm" || ct == "PartTime" || ct == "Temporary")
            .WithMessage("Geçerli bir sözleşme türü seçilmelidir.");
    }
}