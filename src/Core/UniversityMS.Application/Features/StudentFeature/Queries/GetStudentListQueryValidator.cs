using FluentValidation;

namespace UniversityMS.Application.Features.StudentFeature.Queries;

public class GetStudentListQueryValidator : AbstractValidator<GetStudentListQuery>
{
    public GetStudentListQueryValidator()
    {
        RuleFor(x => x.PageNumber)
            .GreaterThan(0).WithMessage("Sayfa numarası 0'dan büyük olmalıdır.");

        RuleFor(x => x.PageSize)
            .GreaterThan(0).WithMessage("Sayfa boyutu 0'dan büyük olmalıdır.")
            .LessThanOrEqualTo(100).WithMessage("Sayfa boyutu en fazla 100 olabilir.");

        RuleFor(x => x.Filter)
            .MaximumLength(500).When(x => !string.IsNullOrEmpty(x.Filter))
            .WithMessage("Filter string'i en fazla 500 karakter olabilir.")
            .Matches(@"^[a-zA-Z0-9|,;.\-]*$").When(x => !string.IsNullOrEmpty(x.Filter))
            .WithMessage("Filter string'i geçerli karakterler içermelidir");
    }
}