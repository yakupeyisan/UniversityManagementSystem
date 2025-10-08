using AutoMapper;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;
using System.Diagnostics;
using UniversityMS.Application.Common.Interfaces;
using UniversityMS.Application.Features.Authentication.DTOs;
using UniversityMS.Application.Features.Staff.DTOs;
using UniversityMS.Application.Features.Students.DTOs;
using UniversityMS.Domain.Entities.IdentityAggregate;
using UniversityMS.Domain.Entities.PersonAggregate;

namespace UniversityMS.Application.Common.Behaviours;


public class ValidationBehaviour<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
{
    private readonly IEnumerable<IValidator<TRequest>> _validators;

    public ValidationBehaviour(IEnumerable<IValidator<TRequest>> validators)
    {
        _validators = validators;
    }

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        if (!_validators.Any())
        {
            return await next();
        }

        var context = new ValidationContext<TRequest>(request);

        var validationResults = await Task.WhenAll(
            _validators.Select(v => v.ValidateAsync(context, cancellationToken)));

        var failures = validationResults
            .Where(r => r.Errors.Any())
            .SelectMany(r => r.Errors)
            .ToList();

        if (failures.Any())
        {
            throw new ValidationException(failures);
        }

        return await next();
    }
}


public class LoggingBehaviour<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
{
    private readonly ILogger<LoggingBehaviour<TRequest, TResponse>> _logger;
    private readonly ICurrentUserService _currentUserService;

    public LoggingBehaviour(ILogger<LoggingBehaviour<TRequest, TResponse>> logger,
        ICurrentUserService currentUserService)
    {
        _logger = logger;
        _currentUserService = currentUserService;
    }

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        var requestName = typeof(TRequest).Name;
        var userId = _currentUserService.UserId;
        var username = _currentUserService.Username;

        _logger.LogInformation("UniversityMS Request: {Name} {@UserId} {@Username} {@Request}",
            requestName, userId, username, request);

        var response = await next();

        _logger.LogInformation("UniversityMS Response: {Name} {@Response}", requestName, response);

        return response;
    }
}


public class PerformanceBehaviour<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
{
    private readonly Stopwatch _timer;
    private readonly ILogger<PerformanceBehaviour<TRequest, TResponse>> _logger;
    private readonly ICurrentUserService _currentUserService;

    public PerformanceBehaviour(
        ILogger<PerformanceBehaviour<TRequest, TResponse>> logger,
        ICurrentUserService currentUserService)
    {
        _timer = new Stopwatch();
        _logger = logger;
        _currentUserService = currentUserService;
    }

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        _timer.Start();

        var response = await next();

        _timer.Stop();

        var elapsedMilliseconds = _timer.ElapsedMilliseconds;

        if (elapsedMilliseconds > 500) // 500ms'den uzun süren requestleri logla
        {
            var requestName = typeof(TRequest).Name;
            var userId = _currentUserService.UserId;
            var username = _currentUserService.Username;

            _logger.LogWarning(
                "UniversityMS Long Running Request: {Name} ({ElapsedMilliseconds} milliseconds) {@UserId} {@Username} {@Request}",
                requestName, elapsedMilliseconds, userId, username, request);
        }

        return response;
    }
}

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        // Student Mappings
        CreateMap<Student, StudentDto>()
            .ForMember(d => d.FullName, opt => opt.MapFrom(s => s.GetFullName()))
            .ForMember(d => d.Age, opt => opt.MapFrom(s => s.GetAge()))
            .ForMember(d => d.Email, opt => opt.MapFrom(s => s.Email.Value))
            .ForMember(d => d.PhoneNumber, opt => opt.MapFrom(s => s.PhoneNumber.Value));

        // Staff Mappings
        CreateMap<Staff, StaffDto>()
            .ForMember(d => d.FullName, opt => opt.MapFrom(s => s.GetFullName()))
            .ForMember(d => d.Age, opt => opt.MapFrom(s => s.GetAge()))
            .ForMember(d => d.Email, opt => opt.MapFrom(s => s.Email.Value))
            .ForMember(d => d.PhoneNumber, opt => opt.MapFrom(s => s.PhoneNumber.Value))
            .ForMember(d => d.YearsOfService, opt => opt.MapFrom(s => s.GetYearsOfService()));

        // User Mappings
        CreateMap<User, UserDto>()
            .ForMember(d => d.Email, opt => opt.MapFrom(s => s.Email.Value))
            .ForMember(d => d.Roles, opt => opt.MapFrom(s => s.UserRoles.Select(ur => ur.Role.Name)));

        // Role Mappings
        CreateMap<Role, RoleDto>();

        // Address Mappings
        CreateMap<Address, AddressDto>();

        // Emergency Contact Mappings
        CreateMap<EmergencyContact, EmergencyContactDto>()
            .ForMember(d => d.PhoneNumber, opt => opt.MapFrom(s => s.PhoneNumber.Value))
            .ForMember(d => d.AlternativePhone, opt => opt.MapFrom(s =>
                s.AlternativePhone != null ? s.AlternativePhone.Value : null));
    }
}
