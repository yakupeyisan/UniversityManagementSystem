using MediatR;
using UniversityMS.Application.Common.Models;

namespace UniversityMS.Application.Features.AuthenticationFeature.Commands;

public record LogoutCommand(Guid UserId) : IRequest<Result>;