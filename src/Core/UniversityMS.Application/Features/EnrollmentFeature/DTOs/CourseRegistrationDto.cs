using UniversityMS.Domain.Enums;

namespace UniversityMS.Application.Features.EnrollmentFeature.DTOs;

public class CourseRegistrationDto
{
    public Guid Id { get; set; }
    public Guid CourseId { get; set; }
    public string CourseName { get; set; } = string.Empty;
    public string CourseCode { get; set; } = string.Empty;
    public int Credits { get; set; }
    public DateTime RegistrationDate { get; set; }
    public string Status { get; set; } = string.Empty;
}