namespace UniversityMS.Application.Features.ScheduleFeature.DTOs;

public class ScheduleConflictDto
{
    public Guid Session1Id { get; set; }
    public Guid Session2Id { get; set; }
    public string ConflictType { get; set; } = string.Empty; // Instructor, Classroom, etc.
    public string Message { get; set; } = string.Empty;
}
