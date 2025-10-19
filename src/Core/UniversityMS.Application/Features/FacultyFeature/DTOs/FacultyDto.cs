namespace UniversityMS.Application.Features.FacultyFeature.DTOs;

public class FacultyDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
    public string? Description { get; set; }
    public bool IsActive { get; set; }
    public int DepartmentCount { get; set; }
    public DateTime CreatedAt { get; set; }
}