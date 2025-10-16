using MediatR;
using UniversityMS.Application.Common.Models;
using UniversityMS.Application.Features.HR.DTOs;

namespace UniversityMS.Application.Features.HR.Commands;

public record UpdateEmployeeCommand(
    Guid EmployeeId,
    string? JobTitle,
    Guid? DepartmentId,
    decimal? BaseSalary,
    TimeOnly? StartTime,
    TimeOnly? EndTime,
    int? WeeklyHours,
    string? Notes
) : IRequest<Result<EmployeeDto>>;