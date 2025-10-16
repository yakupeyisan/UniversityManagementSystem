using MediatR;
using UniversityMS.Application.Common.Models;
using UniversityMS.Application.Features.HR.DTOs;
using UniversityMS.Domain.Entities.HRAggregate;
using UniversityMS.Domain.Interfaces;

namespace UniversityMS.Application.Features.HR.Queries;

public class GetEmployeesByDepartmentQueryHandler : IRequestHandler<GetEmployeesByDepartmentQuery, Result<List<EmployeeListDto>>>
{
    private readonly IRepository<Employee> _employeeRepository;

    public GetEmployeesByDepartmentQueryHandler(
        IRepository<Employee> employeeRepository)
    {
        _employeeRepository = employeeRepository;
    }

    public async Task<Result<List<EmployeeListDto>>> Handle(
        GetEmployeesByDepartmentQuery request,
        CancellationToken cancellationToken)
    {
        var employees = await _employeeRepository.GetAllAsync(cancellationToken);
        var departmentEmployees = employees
            .Where(e => e.DepartmentId == request.DepartmentId)
            .OrderBy(e => e.Person.LastName)
            .ToList();

        var dtos = departmentEmployees.Select(e => new EmployeeListDto(
            e.Id,
            e.EmployeeNumber.Value,
            $"{e.Person.FirstName} {e.Person.LastName}",
            e.Person.Email.Value,
            e.JobTitle,
            e.Department?.Name,
            e.Status.ToString()
        )).ToList();

        return Result<List<EmployeeListDto>>.Success(dtos);
    }
}