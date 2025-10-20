using MediatR;
using UniversityMS.Application.Common.Models;
using UniversityMS.Application.Features.HRFeature.DTOs;
using UniversityMS.Domain.Entities.HRAggregate;
using UniversityMS.Domain.Interfaces;
using UniversityMS.Domain.Specifications;

namespace UniversityMS.Application.Features.HRFeature.Queries;

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
        var spec = new EmployeesByDepartmentSpecification(request.DepartmentId);

        var employees = await _employeeRepository.ListAsync(spec, cancellationToken);

        var dtos = employees.Select(e => new EmployeeListDto(
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