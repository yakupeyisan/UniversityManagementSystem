using MediatR;
using Microsoft.Extensions.Logging;
using UniversityMS.Application.Common.Models;
using UniversityMS.Domain.Entities.IdentityAggregate;
using UniversityMS.Domain.Exceptions;
using UniversityMS.Domain.Interfaces;

namespace UniversityMS.Application.Features.AuthenticationFeature.Commands;

public class LogoutCommandHandler : IRequestHandler<LogoutCommand, Result>
{
    private readonly IRepository<User> _userRepository;
    private readonly ILogger<LogoutCommandHandler> _logger;

    public LogoutCommandHandler(
        IRepository<User> userRepository,
        ILogger<LogoutCommandHandler> logger)
    {
        _userRepository = userRepository;
        _logger = logger;
    }

    public async Task<Result> Handle(LogoutCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var user = await _userRepository.GetByIdAsync(request.UserId, cancellationToken);

            if (user == null)
            {
                throw new EntityNotFoundException(nameof(User), request.UserId);
            }

            // Refresh token'ı temizle
            user.ClearRefreshToken();

            await _userRepository.UpdateAsync(user, cancellationToken);

            _logger.LogInformation("User logged out successfully: {UserId}", user.Id);
            return Result.Success("Çıkış başarılı.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred during logout for user: {UserId}", request.UserId);
            return Result.Failure("Çıkış sırasında bir hata oluştu.");
        }
    }
}