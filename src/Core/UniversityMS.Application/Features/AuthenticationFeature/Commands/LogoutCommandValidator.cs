using FluentValidation;

namespace UniversityMS.Application.Features.AuthenticationFeature.Commands;

public class LogoutCommandValidator : AbstractValidator<LogoutCommand>
{
    public LogoutCommandValidator()
    {
        RuleFor(x => x.UserId)
            .NotEmpty().WithMessage("Kullanıcı ID gereklidir.")
            .NotEqual(Guid.Empty).WithMessage("Kullanıcı ID boş GUID olamaz.");
    }
}