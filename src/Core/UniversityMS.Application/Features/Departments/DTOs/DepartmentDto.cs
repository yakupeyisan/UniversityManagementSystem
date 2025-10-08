using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UniversityMS.Application.Features.Departments.DTOs;


public class DepartmentDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
    public Guid FacultyId { get; set; }
    public string FacultyName { get; set; } = string.Empty;
    public string? Description { get; set; }
    public Guid? HeadOfDepartmentId { get; set; }
    public bool IsActive { get; set; }
    public int CourseCount { get; set; }
    public DateTime CreatedAt { get; set; }
}