using UniversityMS.Domain.Enums;

namespace UniversityMS.Application.Features.Enrollments.DTOs;

public class EnrollmentDto
{
    public Guid Id { get; set; }
    public Guid StudentId { get; set; }
    public string AcademicYear { get; set; } = string.Empty;
    public int Semester { get; set; }
    public EnrollmentStatus Status { get; set; }
    public DateTime EnrollmentDate { get; set; }
    public DateTime? ApprovalDate { get; set; }
    public Guid? ApprovedBy { get; set; }
    public int TotalECTS { get; set; }
    public int TotalNationalCredit { get; set; }
    public List<CourseRegistrationDto> CourseRegistrations { get; set; } = new();
}
