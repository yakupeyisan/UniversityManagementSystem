using MediatR;
using UniversityMS.Application.Common.Models;

namespace UniversityMS.Application.Features.Authentication.Commands;

public record RegisterCommand(
    string Username,
    string Email,
    string Password,
    string? FirstName,
    string? LastName
) : IRequest<Result<Guid>>;