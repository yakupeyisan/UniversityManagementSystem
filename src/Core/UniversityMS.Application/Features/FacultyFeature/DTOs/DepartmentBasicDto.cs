namespace UniversityMS.Application.Features.FacultyFeature.DTOs;

public class DepartmentBasicDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
    public bool IsActive { get; set; }
}