using MediatR;
using UniversityMS.Application.Common.Models;
using UniversityMS.Application.Features.Authentication.DTOs;

namespace UniversityMS.Application.Features.Authentication.Commands;
public record LoginCommand(string Username, string Password) : IRequest<Result<TokenDto>>;