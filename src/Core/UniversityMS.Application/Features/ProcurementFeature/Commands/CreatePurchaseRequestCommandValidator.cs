using FluentValidation;

namespace UniversityMS.Application.Features.ProcurementFeature.Commands;

public class CreatePurchaseRequestCommandValidator : AbstractValidator<CreatePurchaseRequestCommand>
{
    public CreatePurchaseRequestCommandValidator()
    {
        RuleFor(x => x.DepartmentId).NotEqual(Guid.Empty);
        RuleFor(x => x.Title).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Items).NotEmpty().WithMessage("En az bir ürün eklenmelidir");
    }
}