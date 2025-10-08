using UniversityMS.Application.Common.Mappings;

namespace UniversityMS.Application.Features.Authentication.DTOs;
public class LoginDto
{
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}
public class TokenDto
{
    public string AccessToken { get; set; } = string.Empty;
    public string RefreshToken { get; set; } = string.Empty;
    public DateTime ExpiresAt { get; set; }
    public UserDto User { get; set; } = null!;
}

public class UserDto
{
    public Guid Id { get; set; }
    public string Username { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public bool IsActive { get; set; }
    public List<string> Roles { get; set; } = new();
}
public class RoleDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
}
public class AddressDto
{
    public string Street { get; set; } = string.Empty;
    public string? BuildingNo { get; set; }
    public string? ApartmentNo { get; set; }
    public string District { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public string PostalCode { get; set; } = string.Empty;
    public string Country { get; set; } = string.Empty;
}

public class EmergencyContactDto
{
    public string FullName { get; set; } = string.Empty;
    public string Relationship { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public string? AlternativePhone { get; set; }
}