using MediatR;
using UniversityMS.Application.Common.Models;
using UniversityMS.Application.Features.Staff.DTOs;
using UniversityMS.Domain.Entities.HRAggregate;
using UniversityMS.Domain.Interfaces;
using UniversityMS.Domain.Specifications;

namespace UniversityMS.Application.Features.Staff.Queries;

public class SearchEmployeesQueryHandler : IRequestHandler<SearchEmployeesQuery, Result<List<EmployeeDto>>>
{
    private readonly IRepository<Employee> _employeeRepository;

    public SearchEmployeesQueryHandler(IRepository<Employee> employeeRepository)
    {
        _employeeRepository = employeeRepository;
    }

    public async Task<Result<List<EmployeeDto>>> Handle(
        SearchEmployeesQuery request,
        CancellationToken cancellationToken)
    {
        var spec = new EmployeeSearchSpecification(
            request.SearchTerm,
            request.DepartmentId,
            request.PageNumber,
            request.PageSize
        );

        var employees = await _employeeRepository.ListAsync(spec, cancellationToken);

        var dtos = employees.Select(e => new EmployeeDto
        {
            Id = e.Id,
            EmployeeNumber = e.EmployeeNumber.Value,
            JobTitle = e.JobTitle,
            Status = e.Status.ToString(),
            HireDate = e.HireDate,
            DepartmentId = e.DepartmentId
        }).ToList();

        return Result<List<EmployeeDto>>.Success(dtos);
    }
}