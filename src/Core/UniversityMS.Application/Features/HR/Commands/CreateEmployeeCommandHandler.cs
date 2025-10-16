using AutoMapper;
using MediatR;
using UniversityMS.Application.Common.Models;
using UniversityMS.Application.Features.HR.DTOs;
using UniversityMS.Domain.Entities.HRAggregate;
using UniversityMS.Domain.Interfaces;
using UniversityMS.Domain.ValueObjects;

namespace UniversityMS.Application.Features.HR.Commands;

public class CreateEmployeeCommandHandler : IRequestHandler<CreateEmployeeCommand, Result<EmployeeDto>>
{
    private readonly IRepository<Employee> _employeeRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public CreateEmployeeCommandHandler(
        IRepository<Employee> employeeRepository,
        IUnitOfWork unitOfWork,
        IMapper mapper)
    {
        _employeeRepository = employeeRepository;
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<Result<EmployeeDto>> Handle(
        CreateEmployeeCommand request,
        CancellationToken cancellationToken)
    {
        // Duplicate check
        var existing = await _employeeRepository.FindAsync(
            e => e.EmployeeNumber.Value == request.EmployeeNumber,
            cancellationToken);

        if (existing.Any())
            return Result.Failure<EmployeeDto>("Bu çalışan numarası zaten kullanılıyor");

        // Create Value Objects using static factory methods
        var employeeNumber = EmployeeNumber.Create(request.EmployeeNumber);
        var salary = SalaryInfo.Create(request.BaseSalary, "TRY");
        var workingHours = WorkingHours.Create(request.StartTime, request.EndTime, request.WeeklyHours);

        // Create Employee using static factory method
        var employee = Employee.Create(
            employeeNumber,
            request.PersonId,
            request.JobTitle,
            request.HireDate,
            salary,
            workingHours,
            request.DepartmentId,
            request.Notes
        );

        await _employeeRepository.AddAsync(employee, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        var dto = _mapper.Map<EmployeeDto>(employee);
        return Result<EmployeeDto>.Success(dto);
    }
}