using MediatR;
using UniversityMS.Application.Common.Models;
using UniversityMS.Application.Features.HRFeature.DTOs;

namespace UniversityMS.Application.Features.HRFeature.Commands;

/// <summary>
/// Çalışan oluşturma command record'ı
/// </summary>
public record CreateEmployeeCommand(
    string EmployeeNumber,
    Guid PersonId,
    string JobTitle,
    DateTime HireDate,
    decimal BaseSalary,
    TimeOnly StartTime,
    TimeOnly EndTime,
    int WeeklyHours,
    Guid? DepartmentId = null,
    string? Notes = null
) : IRequest<Result<EmployeeDto>>;