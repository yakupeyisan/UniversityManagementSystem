using MediatR;
using UniversityMS.Application.Common.Models;

namespace UniversityMS.Application.Features.Authentication.Commands;

public record LogoutCommand(Guid UserId) : IRequest<Result>;