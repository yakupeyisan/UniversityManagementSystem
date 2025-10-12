namespace UniversityMS.Application.Features.Authentication.DTOs;

public class EmergencyContactDto
{
    public string FullName { get; set; } = string.Empty;
    public string Relationship { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public string? AlternativePhone { get; set; }
}