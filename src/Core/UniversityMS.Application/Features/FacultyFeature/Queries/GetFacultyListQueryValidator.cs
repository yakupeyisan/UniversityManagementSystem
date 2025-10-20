using FluentValidation;

namespace UniversityMS.Application.Features.FacultyFeature.Queries;

public class GetFacultyListQueryValidator : AbstractValidator<GetFacultyListQuery>
{
    public GetFacultyListQueryValidator()
    {
        RuleFor(x => x.PageNumber).GreaterThan(0);
        RuleFor(x => x.PageSize).GreaterThan(0).LessThanOrEqualTo(100);
        RuleFor(x => x.Filter).MaximumLength(500).When(x => !string.IsNullOrEmpty(x.Filter));
    }
}