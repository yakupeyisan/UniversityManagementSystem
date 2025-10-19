using UniversityMS.Domain.Enums;

namespace UniversityMS.Application.Features.EnrollmentFeature.DTOs;

public class EnrollmentDto
{
    public Guid Id { get; set; }
    public Guid StudentId { get; set; }
    public string AcademicYear { get; set; } = string.Empty;
    public int Semester { get; set; }
    public EnrollmentStatus Status { get; set; }
    public DateTime EnrollmentDate { get; set; }
    public DateTime? ApprovedDate { get; set; }
    public DateTime? DroppedDate { get; set; }
}

