using MediatR;
using UniversityMS.Application.Common.Models;
using UniversityMS.Application.Features.AuthenticationFeature.DTOs;
using UniversityMS.Domain.Enums;

namespace UniversityMS.Application.Features.AuthenticationFeature.Commands;
public record RegisterCommand : IRequest<Result<UserDto>>
{
    public string Username { get; init; } = string.Empty;
    public string Password { get; init; } = string.Empty;
    public string Email { get; init; } = string.Empty;
    public string FirstName { get; init; } = string.Empty;
    public string LastName { get; init; } = string.Empty;
    public string NationalId { get; init; } = string.Empty;
    public DateTime BirthDate { get; init; }
    public Gender Gender { get; init; }
    public string PhoneNumber { get; init; } = string.Empty;
    public string? UserType { get; init; } // "Student", "Staff", "Faculty"
}