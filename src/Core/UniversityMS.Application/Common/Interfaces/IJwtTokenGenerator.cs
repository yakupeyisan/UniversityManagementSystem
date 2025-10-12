using System.Security.Claims;
using UniversityMS.Domain.Entities.IdentityAggregate;

namespace UniversityMS.Application.Common.Interfaces;

public interface IJwtTokenGenerator
{
    string GenerateToken(User user);
    string GenerateRefreshToken();
    ClaimsPrincipal? GetPrincipalFromExpiredToken(string token);
}