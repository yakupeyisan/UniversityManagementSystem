using MediatR;
using UniversityMS.Application.Common.Models;
using UniversityMS.Application.Features.HR.DTOs;
using UniversityMS.Domain.Entities.HRAggregate;
using UniversityMS.Domain.Interfaces;
using UniversityMS.Domain.ValueObjects;

namespace UniversityMS.Application.Features.Staff.Commands;

public class CreateEmployeeCommandHandler : IRequestHandler<CreateEmployeeCommand, Result<EmployeeDto>>
{
    private readonly IRepository<Employee> _employeeRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateEmployeeCommandHandler(
        IRepository<Employee> employeeRepository,
        IUnitOfWork unitOfWork)
    {
        _employeeRepository = employeeRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<EmployeeDto>> Handle(
        CreateEmployeeCommand request,
        CancellationToken cancellationToken)
    {
        var salary = SalaryInfo.Create(request.BaseSalary);
        var workingHours = WorkingHours.Create(request.WorkingHoursPerWeek);
        var leaveBalance = LeaveBalance.Create(20, 10); // 20 Annual, 10 Sick

        var employee = Employee.Create(
            EmployeeNumber.Create(request.EmployeeNumber),
            request.PersonId,
            request.JobTitle,
            request.HireDate,
            salary,
            workingHours,
            request.DepartmentId
        );

        await _employeeRepository.AddAsync(employee, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result<EmployeeDto>.Success(new EmployeeDto
        {
            Id = employee.Id,
            EmployeeNumber = employee.EmployeeNumber.Value,
            JobTitle = employee.JobTitle,
            Status = employee.Status.ToString(),
            HireDate = employee.HireDate
        });
    }
}