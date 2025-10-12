﻿using FluentValidation;

namespace UniversityMS.Application.Features.Authentication.Commands;

public class RefreshTokenCommandValidator : AbstractValidator<RefreshTokenCommand>
{
    public RefreshTokenCommandValidator()
    {
        RuleFor(x => x.AccessToken)
            .NotEmpty().WithMessage("Access token boş olamaz.");

        RuleFor(x => x.RefreshToken)
            .NotEmpty().WithMessage("Refresh token boş olamaz.");
    }
}