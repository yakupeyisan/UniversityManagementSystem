using FluentValidation;

namespace UniversityMS.Application.Features.DepartmentFeature.Queries;

public class GetDepartmentListQueryValidator : AbstractValidator<GetDepartmentListQuery>
{
    public GetDepartmentListQueryValidator()
    {
        RuleFor(x => x.PageNumber).GreaterThan(0);
        RuleFor(x => x.PageSize).GreaterThan(0).LessThanOrEqualTo(100);
        RuleFor(x => x.Filter).MaximumLength(500).When(x => !string.IsNullOrEmpty(x.Filter));
    }
}