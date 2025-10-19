using UniversityMS.Domain.Enums;

namespace UniversityMS.Application.Features.CourseFeature.DTOs;

public class CourseDto
{
    public Guid Id { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public int Credits { get; set; }
    public int Semester { get; set; }
    public Guid DepartmentId { get; set; }
    public string DepartmentName { get; set; } = string.Empty;
    public bool IsActive { get; set; }
    public int StudentCount { get; set; }
}
