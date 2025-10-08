using AutoMapper;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;
using System.Security.Claims;
using UniversityMS.Application.Common.Interfaces;
using UniversityMS.Application.Common.Models;
using UniversityMS.Application.Features.Authentication.DTOs;
using UniversityMS.Domain.Entities.IdentityAggregate;
using UniversityMS.Domain.Exceptions;
using UniversityMS.Domain.Interfaces;
using UniversityMS.Domain.Specifications;

namespace UniversityMS.Application.Features.Authentication.Commands;
public record LoginCommand(string Username, string Password) : IRequest<Result<TokenDto>>;
public class LoginCommandValidator : AbstractValidator<LoginCommand>
{
    public LoginCommandValidator()
    {
        RuleFor(x => x.Username)
            .NotEmpty().WithMessage("Kullanıcı adı boş olamaz.")
            .MinimumLength(3).WithMessage("Kullanıcı adı en az 3 karakter olmalıdır.");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Şifre boş olamaz.")
            .MinimumLength(6).WithMessage("Şifre en az 6 karakter olmalıdır.");
    }
}

public class LoginCommandHandler : IRequestHandler<LoginCommand, Result<TokenDto>>
{
    private readonly IRepository<Domain.Entities.IdentityAggregate.User> _userRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IJwtTokenGenerator _tokenGenerator;
    private readonly IMapper _mapper;
    private readonly ILogger<LoginCommandHandler> _logger;

    public LoginCommandHandler(
        IRepository<Domain.Entities.IdentityAggregate.User> userRepository,
        IUnitOfWork unitOfWork,
        IPasswordHasher passwordHasher,
        IJwtTokenGenerator tokenGenerator,
        IMapper mapper,
        ILogger<LoginCommandHandler> logger)
    {
        _userRepository = userRepository;
        _unitOfWork = unitOfWork;
        _passwordHasher = passwordHasher;
        _tokenGenerator = tokenGenerator;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<Result<TokenDto>> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        try
        {
            // Kullanıcıyı bul
            var spec = new UserByUsernameSpec(request.Username);
            var user = await _userRepository.GetBySpecAsync(spec, cancellationToken);

            if (user == null)
            {
                _logger.LogWarning("Login failed. User not found: {Username}", request.Username);
                return Result.Failure<TokenDto>("Kullanıcı adı veya şifre hatalı.");
            }

            if (!user.IsActive)
            {
                _logger.LogWarning("Login failed. User is inactive: {Username}", request.Username);
                return Result.Failure<TokenDto>("Hesabınız aktif değil. Lütfen yönetici ile iletişime geçin.");
            }

            // Şifre kontrolü
            if (!_passwordHasher.VerifyPassword(user.PasswordHash, request.Password))
            {
                _logger.LogWarning("Login failed. Invalid password for user: {Username}", request.Username);
                return Result.Failure<TokenDto>("Kullanıcı adı veya şifre hatalı.");
            }

            // Token oluştur
            var accessToken = _tokenGenerator.GenerateToken(user);
            var refreshToken = _tokenGenerator.GenerateRefreshToken();

            // Refresh token'ı kaydet
            user.SetRefreshToken(refreshToken, DateTime.UtcNow.AddDays(7));
            user.UpdateLastLogin();

            await _userRepository.UpdateAsync(user, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            var userDto = _mapper.Map<UserDto>(user);

            var tokenDto = new TokenDto
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken,
                ExpiresAt = DateTime.UtcNow.AddMinutes(60), // from settings
                User = userDto
            };

            _logger.LogInformation("User logged in successfully: {Username}", request.Username);
            return Result.Success(tokenDto, "Giriş başarılı.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred during login for user: {Username}", request.Username);
            return Result.Failure<TokenDto>("Giriş sırasında bir hata oluştu.", ex.Message);
        }
    }
}
public record RegisterCommand(
    string Username,
    string Email,
    string Password,
    string? FirstName,
    string? LastName
) : IRequest<Result<Guid>>;

public class RegisterCommandValidator : AbstractValidator<RegisterCommand>
{
    public RegisterCommandValidator()
    {
        RuleFor(x => x.Username)
            .NotEmpty().WithMessage("Kullanıcı adı boş olamaz.")
            .MinimumLength(3).WithMessage("Kullanıcı adı en az 3 karakter olmalıdır.")
            .MaximumLength(50).WithMessage("Kullanıcı adı en fazla 50 karakter olabilir.");

        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("E-posta adresi boş olamaz.")
            .EmailAddress().WithMessage("Geçerli bir e-posta adresi giriniz.");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Şifre boş olamaz.")
            .MinimumLength(6).WithMessage("Şifre en az 6 karakter olmalıdır.")
            .Matches("[A-Z]").WithMessage("Şifre en az bir büyük harf içermelidir.")
            .Matches("[a-z]").WithMessage("Şifre en az bir küçük harf içermelidir.")
            .Matches("[0-9]").WithMessage("Şifre en az bir rakam içermelidir.");
    }
}
public record RefreshTokenCommand(string AccessToken, string RefreshToken) : IRequest<Result<TokenDto>>;

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
                return Result.Failure<TokenDto>("Geçersiz token.");
            }

            var userIdClaim = principal.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
            {
                _logger.LogWarning("Invalid user ID in token");
                return Result.Failure<TokenDto>("Geçersiz kullanıcı bilgisi.");
            }

            // User'ı bul ve refresh token'ı kontrol et
            var user = await _userRepository.GetByIdAsync(userId, cancellationToken);

            if (user == null)
            {
                _logger.LogWarning("User not found: {UserId}", userId);
                return Result.Failure<TokenDto>("Kullanıcı bulunamadı.");
            }

            if (user.RefreshToken != request.RefreshToken)
            {
                _logger.LogWarning("Invalid refresh token for user: {UserId}", userId);
                return Result.Failure<TokenDto>("Geçersiz refresh token.");
            }

            if (!user.IsActive)
            {
                _logger.LogWarning("User is not active: {UserId}", user.Id);
                return Result.Failure<TokenDto>("Hesabınız aktif değil.");
            }

            // Refresh token'ın süresini kontrol et
            if (user.RefreshTokenExpiryTime == null || user.RefreshTokenExpiryTime <= DateTime.UtcNow)
            {
                _logger.LogWarning("Refresh token expired for user: {UserId}", user.Id);
                return Result.Failure<TokenDto>("Refresh token süresi dolmuş. Lütfen tekrar giriş yapın.");
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
            return Result.Success(tokenDto, "Token yenilendi.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred during refresh token");
            return Result.Failure<TokenDto>("Token yenileme sırasında bir hata oluştu.", ex.Message);
        }
    }
}
public record LogoutCommand(Guid UserId) : IRequest<Result>;

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