using MediatR;
using UniversityMS.Application.Common.Models;
using UniversityMS.Application.Features.AuthenticationFeature.DTOs;

namespace UniversityMS.Application.Features.AuthenticationFeature.Commands;

public record RefreshTokenCommand(string AccessToken, string RefreshToken) : IRequest<Result<TokenDto>>;