using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using UniversityMS.Application.Common.Interfaces;
using UniversityMS.Application.Common.Models;
using UniversityMS.Application.Features.AuthenticationFeature.DTOs;
using UniversityMS.Domain.Entities.IdentityAggregate;
using UniversityMS.Domain.Interfaces;
using UniversityMS.Domain.Specifications;
using UniversityMS.Domain.ValueObjects;

namespace UniversityMS.Application.Features.AuthenticationFeature.Commands;

public class RegisterCommandHandler : IRequestHandler<RegisterCommand, Result<UserDto>>
{
    private readonly IRepository<User> _userRepository;
    private readonly IRepository<Role> _roleRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IMapper _mapper;
    private readonly ILogger<RegisterCommandHandler> _logger;

    public RegisterCommandHandler(
        IRepository<User> userRepository,
        IRepository<Role> roleRepository,
        IUnitOfWork unitOfWork,
        IPasswordHasher passwordHasher,
        IMapper mapper,
        ILogger<RegisterCommandHandler> logger)
    {
        _userRepository = userRepository;
        _roleRepository = roleRepository;
        _unitOfWork = unitOfWork;
        _passwordHasher = passwordHasher;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<Result<UserDto>> Handle(RegisterCommand request, CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("Starting user registration for username: {Username}", request.Username);

            // 1. Kullanıcı adı kontrolü
            var usernameSpec = new UserByUsernameSpec(request.Username);
            var existingUser = await _userRepository.GetBySpecAsync(usernameSpec, cancellationToken);

            if (existingUser != null)
            {
                _logger.LogWarning("Registration failed. Username already exists: {Username}", request.Username);
                return Result<UserDto>.Failure("Bu kullanıcı adı zaten kullanılıyor.");
            }

            // 2. Email kontrolü
            var emailSpec = new UserByEmailSpec(request.Email);
            existingUser = await _userRepository.GetBySpecAsync(emailSpec, cancellationToken);

            if (existingUser != null)
            {
                _logger.LogWarning("Registration failed. Email already exists: {Email}", request.Email);
                return Result<UserDto>.Failure("Bu email adresi zaten kayıtlı.");
            }

            // 3. TC Kimlik No kontrolü
            var nationalIdSpec = new UserByNationalIdSpec(request.NationalId);
            existingUser = await _userRepository.GetBySpecAsync(nationalIdSpec, cancellationToken);

            if (existingUser != null)
            {
                _logger.LogWarning("Registration failed. National ID already exists: {NationalId}", request.NationalId);
                return Result<UserDto>.Failure("Bu TC Kimlik No zaten kayıtlı.");
            }

            // 4. Value Objects oluştur
            var email = Email.Create(request.Email);
            var phoneNumber = PhoneNumber.Create(request.PhoneNumber);

            // 5. Şifreyi hashle
            var passwordHash = _passwordHasher.HashPassword(request.Password);

            // 6. User entity oluştur
            var user = User.Create(
                request.Username,
                email,
                passwordHash,
                request.FirstName,
                request.LastName
            );

            // 7. Varsayılan rol ata (Student, Staff, Faculty)
            var roleSpec = new RoleByNameSpec(request.UserType ?? "Student");
            var defaultRole = await _roleRepository.GetBySpecAsync(roleSpec, cancellationToken);

            if (defaultRole == null)
            {
                _logger.LogWarning("Default role not found: {UserType}", request.UserType);
                return Result<UserDto>.Failure("Varsayılan rol bulunamadı.");
            }

            var userRole = UserRole.Create(user.Id, defaultRole.Id);

            // 8. Veritabanına kaydet
            await _userRepository.AddAsync(user, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("User registered successfully: {Username}, ID: {UserId}", user.Username, user.Id);

            // 9. Response oluştur
            var userDto = _mapper.Map<UserDto>(user);
            return Result<UserDto>.Success(userDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while registering user: {Username}", request.Username);
            return Result<UserDto>.Failure("Kayıt sırasında bir hata oluştu.");
        }
    }
}