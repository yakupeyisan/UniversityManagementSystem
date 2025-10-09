using UniversityMS.Domain.Enums;

namespace UniversityMS.Application.Features.Courses.DTOs;

public class CourseDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
    public string? Description { get; set; }
    public Guid DepartmentId { get; set; }
    public string DepartmentName { get; set; } = string.Empty;
    public CourseType CourseType { get; set; }
    public int TheoreticalHours { get; set; }
    public int PracticalHours { get; set; }
    public int ECTS { get; set; }
    public int NationalCredit { get; set; }
    public EducationLevel EducationLevel { get; set; }
    public int? Semester { get; set; }
    public bool IsActive { get; set; }
    public int TotalWeeklyHours { get; set; }
    public List<PrerequisiteCourseDto> Prerequisites { get; set; } = new();
}