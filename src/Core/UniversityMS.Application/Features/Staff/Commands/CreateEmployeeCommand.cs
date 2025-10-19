using MediatR;
using UniversityMS.Application.Common.Models;
using UniversityMS.Application.Features.HR.DTOs;

namespace UniversityMS.Application.Features.Staff.Commands;


public class CreateEmployeeCommand : IRequest<Result<EmployeeDto>>
{
    public Guid PersonId { get; set; }
    public string EmployeeNumber { get; set; } = string.Empty;
    public Guid? DepartmentId { get; set; }
    public string JobTitle { get; set; } = string.Empty;
    public DateTime HireDate { get; set; }
    public decimal BaseSalary { get; set; }
    public decimal WorkingHoursPerWeek { get; set; }
}