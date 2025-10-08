using UniversityMS.Domain.Enums;

namespace UniversityMS.Application.Features.Enrollments.DTOs;

public class CourseRegistrationDto
{
    public Guid Id { get; set; }
    public Guid CourseId { get; set; }
    public string CourseName { get; set; } = string.Empty;
    public string CourseCode { get; set; } = string.Empty;
    public int ECTS { get; set; }
    public int NationalCredit { get; set; }
    public CourseRegistrationStatus Status { get; set; }
    public DateTime RegistrationDate { get; set; }
}