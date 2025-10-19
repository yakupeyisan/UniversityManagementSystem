using MediatR;
using UniversityMS.Application.Common.Models;
using UniversityMS.Application.Features.Staff.DTOs;

namespace UniversityMS.Application.Features.Staff.Queries;

public class SearchEmployeesQuery : IRequest<Result<List<EmployeeDto>>>
{
    public string? SearchTerm { get; set; }
    public Guid? DepartmentId { get; set; }
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 20;
}