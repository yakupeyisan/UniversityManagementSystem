namespace UniversityMS.Application.Features.DepartmentFeature.DTOs;

public class DepartmentDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
    public Guid FacultyId { get; set; }
    public string FacultyName { get; set; } = string.Empty;
    public Guid? HeadId { get; set; }
    public string? HeadName { get; set; }
    public string? Description { get; set; }
    public int CourseCount { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
}