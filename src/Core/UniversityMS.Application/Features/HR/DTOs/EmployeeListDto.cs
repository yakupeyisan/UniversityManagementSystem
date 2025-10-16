namespace UniversityMS.Application.Features.HR.DTOs;

public record EmployeeListDto(
    Guid Id,
    string EmployeeNumber,
    string FullName,
    string Email,
    string JobTitle,
    string? DepartmentName,
    string Status
);