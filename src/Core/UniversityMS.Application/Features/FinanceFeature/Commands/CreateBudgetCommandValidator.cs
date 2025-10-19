using FluentValidation;

namespace UniversityMS.Application.Features.FinanceFeature.Commands;

public class CreateBudgetCommandValidator : AbstractValidator<CreateBudgetCommand>
{
    public CreateBudgetCommandValidator()
    {
        RuleFor(x => x.DepartmentId).NotEqual(Guid.Empty);
        RuleFor(x => x.Year).GreaterThanOrEqualTo(2020);
        RuleFor(x => x.TotalAmount).GreaterThan(0);
        RuleFor(x => x.BudgetType).NotEmpty().Must(BeValidBudgetType);
    }

    private bool BeValidBudgetType(string type) =>
        new[] { "Personnel", "Operations", "Capital", "Maintenance" }.Contains(type);
}