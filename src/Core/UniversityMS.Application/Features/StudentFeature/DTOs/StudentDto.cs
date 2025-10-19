using UniversityMS.Domain.Enums;

namespace UniversityMS.Application.Features.StudentFeature.DTOs;

public class StudentDto
{
    public Guid Id { get; set; }
    public string StudentNumber { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public int Age { get; set; }
    public Gender Gender { get; set; }
    public EducationLevel EducationLevel { get; set; }
    public int CurrentSemester { get; set; }
    public StudentStatus Status { get; set; }
    public double CGPA { get; set; }
    public double SGPA { get; set; }
    public int TotalCredits { get; set; }
    public int CompletedCredits { get; set; }
    public string? ProfilePhotoUrl { get; set; }
    public decimal Balance { get; set; }
}