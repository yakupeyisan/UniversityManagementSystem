using MediatR;
using UniversityMS.Application.Common.Models;
using UniversityMS.Application.Features.HRFeature.DTOs;
using UniversityMS.Domain.Entities.HRAggregate;
using UniversityMS.Domain.Enums;
using UniversityMS.Domain.Interfaces;

namespace UniversityMS.Application.Features.HRFeature.Queries;

public class GetActiveEmployeesQueryHandler : IRequestHandler<GetActiveEmployeesQuery, Result<PaginatedList<EmployeeListDto>>>
{
    private readonly IRepository<Employee> _employeeRepository;

    public GetActiveEmployeesQueryHandler(IRepository<Employee> employeeRepository)
    {
        _employeeRepository = employeeRepository;
    }

    public async Task<Result<PaginatedList<EmployeeListDto>>> Handle(
        GetActiveEmployeesQuery request,
        CancellationToken cancellationToken)
    {
        var employees = await _employeeRepository.GetAllAsync(cancellationToken);
        var activeEmployees = employees
            .Where(e => e.Status == EmploymentStatus.Active) // Assuming this enum exists
            .OrderBy(e => e.Person.LastName)
            .ToList();

        var totalCount = activeEmployees.Count();
        var items = activeEmployees
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToList();

        var dtos = items.Select(e => new EmployeeListDto(
            e.Id,
            e.EmployeeNumber.Value,
            $"{e.Person.FirstName} {e.Person.LastName}",
            e.Person.Email.Value,
            e.JobTitle,
            e.Department?.Name,
            e.Status.ToString()
        )).ToList();

        var paginatedList = new PaginatedList<EmployeeListDto>(
            dtos,
            totalCount,
            request.PageNumber,
            request.PageSize
        );

        return Result<PaginatedList<EmployeeListDto>>.Success(paginatedList);
    }
}