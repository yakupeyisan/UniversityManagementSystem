using AutoMapper;
using MediatR;
using UniversityMS.Application.Common.Models;
using UniversityMS.Application.Features.HRFeature.DTOs;
using UniversityMS.Domain.Entities.HRAggregate;
using UniversityMS.Domain.Interfaces;

namespace UniversityMS.Application.Features.HRFeature.Queries;

public class GetAllEmployeesQueryHandler : IRequestHandler<GetAllEmployeesQuery, Result<PaginatedList<EmployeeListDto>>>
{
    private readonly IRepository<Employee> _employeeRepository;
    private readonly IMapper _mapper;

    public GetAllEmployeesQueryHandler(
        IRepository<Employee> employeeRepository,
        IMapper mapper)
    {
        _employeeRepository = employeeRepository;
        _mapper = mapper;
    }

    public async Task<Result<PaginatedList<EmployeeListDto>>> Handle(
        GetAllEmployeesQuery request,
        CancellationToken cancellationToken)
    {
        var query = await _employeeRepository.GetAllAsync(cancellationToken);

        if (!string.IsNullOrEmpty(request.SearchTerm))
            query = query.Where(e =>
                e.EmployeeNumber.Value.Contains(request.SearchTerm) ||
                e.Person.FirstName.Contains(request.SearchTerm) ||
                e.Person.LastName.Contains(request.SearchTerm) ||
                e.Person.Email.Value.Contains(request.SearchTerm)).ToList();

        if (request.DepartmentId.HasValue && request.DepartmentId != Guid.Empty)
            query = query.Where(e => e.DepartmentId == request.DepartmentId).ToList();

        if (!string.IsNullOrEmpty(request.Status))
            query = query.Where(e => e.Status.ToString() == request.Status).ToList();

        var totalCount = query.Count();
        var items = query
            .OrderBy(e => e.Person.LastName)
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