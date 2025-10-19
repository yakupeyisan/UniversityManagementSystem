using MediatR;
using UniversityMS.Application.Common.Models;
using UniversityMS.Application.Features.HR.DTOs;
using UniversityMS.Domain.Entities.HRAggregate;
using UniversityMS.Domain.Interfaces;

namespace UniversityMS.Application.Features.Staff.Queries;

public class GetEmployeeByIdQueryHandler : IRequestHandler<GetEmployeeByIdQuery, Result<EmployeeDto>>
{
    private readonly IRepository<Employee> _employeeRepository;

    public GetEmployeeByIdQueryHandler(IRepository<Employee> employeeRepository)
    {
        _employeeRepository = employeeRepository;
    }

    public async Task<Result<EmployeeDto>> Handle(
        GetEmployeeByIdQuery request,
        CancellationToken cancellationToken)
    {
        var employee = await _employeeRepository.GetByIdAsync(request.EmployeeId, cancellationToken);
        if (employee == null)
            return Result<EmployeeDto>.Failure("Çalışan bulunamadı");

        return Result<EmployeeDto>.Success(new EmployeeDto
        {
            Id = employee.Id,
            EmployeeNumber = employee.EmployeeNumber.Value,
            JobTitle = employee.JobTitle,
            Status = employee.Status.ToString(),
            HireDate = employee.HireDate,
            DepartmentId = employee.DepartmentId
        });
    }
}