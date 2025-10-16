using System.Security.Claims;
using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using UniversityMS.Application.Common.Interfaces;
using UniversityMS.Application.Common.Models;
using UniversityMS.Application.Features.Authentication.DTOs;
using UniversityMS.Domain.Entities.IdentityAggregate;
using UniversityMS.Domain.Interfaces;

namespace UniversityMS.Application.Features.Authentication.Commands;

public class RefreshTokenCommandHandler : IRequestHandler<RefreshTokenCommand, Result<TokenDto>>
{
    private readonly IRepository<User> _userRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IJwtTokenGenerator _tokenGenerator;
    private readonly IMapper _mapper;
    private readonly ILogger<RefreshTokenCommandHandler> _logger;

    public RefreshTokenCommandHandler(
        IRepository<User> userRepository,
        IUnitOfWork unitOfWork,
        IJwtTokenGenerator tokenGenerator,
        IMapper mapper,
        ILogger<RefreshTokenCommandHandler> logger)
    {
        _userRepository = userRepository;
        _unitOfWork = unitOfWork;
        _tokenGenerator = tokenGenerator;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<Result<TokenDto>> Handle(RefreshTokenCommand request, CancellationToken cancellationToken)
    {
        try
        {
            // Expired token'dan principal bilgisini al
            var principal = _tokenGenerator.GetPrincipalFromExpiredToken(request.AccessToken);

            if (principal == null)
            {
                _logger.LogWarning("Invalid access token");
                return Result<TokenDto>.Failure("Geçersiz token.");
            }

            var userIdClaim = principal.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
            {
                _logger.LogWarning("Invalid user ID in token");
                return Result<TokenDto>.Failure("Geçersiz kullanıcı bilgisi.");
            }

            // User'ı bul ve refresh token'ı kontrol et
            var user = await _userRepository.GetByIdAsync(userId, cancellationToken);

            if (user == null)
            {
                _logger.LogWarning("User not found: {UserId}", userId);
                return Result<TokenDto>.Failure("Kullanıcı bulunamadı.");
            }

            if (user.RefreshToken != request.RefreshToken)
            {
                _logger.LogWarning("Invalid refresh token for user: {UserId}", userId);
                return Result<TokenDto>.Failure("Geçersiz refresh token.");
            }

            if (!user.IsActive)
            {
                _logger.LogWarning("User is not active: {UserId}", user.Id);
                return Result<TokenDto>.Failure("Hesabınız aktif değil.");
            }

            // Refresh token'ın süresini kontrol et
            if (user.RefreshTokenExpiryTime == null || user.RefreshTokenExpiryTime <= DateTime.UtcNow)
            {
                _logger.LogWarning("Refresh token expired for user: {UserId}", user.Id);
                return Result<TokenDto>.Failure("Refresh token süresi dolmuş. Lütfen tekrar giriş yapın.");
            }

            // Yeni token'lar oluştur
            var newAccessToken = _tokenGenerator.GenerateToken(user);
            var newRefreshToken = _tokenGenerator.GenerateRefreshToken();

            // User'ın refresh token'ını güncelle
            user.SetRefreshToken(newRefreshToken, DateTime.UtcNow.AddDays(7));
            user.UpdateLastLogin();

            await _userRepository.UpdateAsync(user, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            var userDto = _mapper.Map<UserDto>(user);

            var tokenDto = new TokenDto
            {
                AccessToken = newAccessToken,
                RefreshToken = newRefreshToken,
                ExpiresAt = DateTime.UtcNow.AddMinutes(60),
                User = userDto
            };

            _logger.LogInformation("Refresh token successful for user: {UserId}", user.Id);
            return Result<TokenDto>.Success(tokenDto, "Token yenilendi.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred during refresh token");
            return Result<TokenDto>.Failure("Token yenileme sırasında bir hata oluştu. Hata: " + ex.Message);
        }
    }
}