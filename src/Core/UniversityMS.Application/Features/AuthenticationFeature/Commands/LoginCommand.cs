using MediatR;
using UniversityMS.Application.Common.Models;
using UniversityMS.Application.Features.AuthenticationFeature.DTOs;

namespace UniversityMS.Application.Features.AuthenticationFeature.Commands;
public record LoginCommand(string Username, string Password) : IRequest<Result<TokenDto>>;