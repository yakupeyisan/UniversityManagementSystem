using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using UniversityMS.Application.Common.Interfaces;
using UniversityMS.Application.Common.Models;
using UniversityMS.Application.Features.Authentication.DTOs;
using UniversityMS.Domain.Interfaces;
using UniversityMS.Domain.Specifications;

namespace UniversityMS.Application.Features.Authentication.Commands;

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