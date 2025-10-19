namespace UniversityMS.Application.Features.HRFeature.DTOs;

public record EmployeeDto(
    Guid Id,
    string EmployeeNumber,
    Guid PersonId,
    string PersonFullName,
    string PersonEmail,
    string PersonPhone,
    string JobTitle,
    Guid? DepartmentId,
    string? DepartmentName,
    DateTime HireDate,
    DateTime? TerminationDate,
    decimal BaseSalary,
    int HoursPerWeek,
    string Status,
    string? Notes,
    DateTime CreatedAt
);