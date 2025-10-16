using MediatR;
using UniversityMS.Application.Common.Models;
using UniversityMS.Application.Features.HR.DTOs;

namespace UniversityMS.Application.Features.HR.Commands;

public record CreateEmployeeCommand(
    Guid PersonId,
    string EmployeeNumber,
    string JobTitle,
    DateTime HireDate,
    decimal BaseSalary,
    TimeOnly StartTime,
    TimeOnly EndTime,
    int WeeklyHours,
    Guid? DepartmentId,
    string? Notes
) : IRequest<Result<EmployeeDto>>;