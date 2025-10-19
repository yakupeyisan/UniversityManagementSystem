namespace UniversityMS.Application.Features.Staff.DTOs;

public class EmployeeDto
{
    public Guid Id { get; set; }
    public string EmployeeNumber { get; set; } = string.Empty;
    public string JobTitle { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public DateTime HireDate { get; set; }
    public Guid? DepartmentId { get; set; }
    public decimal BaseSalary { get; set; }
    public DateTime? TerminationDate { get; set; }
}