using AutoMapper;
using MediatR;
using UniversityMS.Application.Common.Models;
using UniversityMS.Application.Features.HR.DTOs;
using UniversityMS.Domain.Entities.HRAggregate;
using UniversityMS.Domain.Interfaces;
using UniversityMS.Domain.ValueObjects;

namespace UniversityMS.Application.Features.HR.Commands;

public class UpdateEmployeeCommandHandler : IRequestHandler<UpdateEmployeeCommand, Result<EmployeeDto>>
{
    private readonly IRepository<Employee> _employeeRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public UpdateEmployeeCommandHandler(
        IRepository<Employee> employeeRepository,
        IUnitOfWork unitOfWork,
        IMapper mapper)
    {
        _employeeRepository = employeeRepository;
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<Result<EmployeeDto>> Handle(
        UpdateEmployeeCommand request,
        CancellationToken cancellationToken)
    {
        var employee = await _employeeRepository.GetByIdAsync(request.EmployeeId, cancellationToken);
        if (employee is null)
            return Result.Failure<EmployeeDto>("Çalışan bulunamadı");

        // Update edilebilir alanlar
        if (!string.IsNullOrEmpty(request.JobTitle))
            employee.UpdateJobTitle(request.JobTitle);

        if (request.DepartmentId.HasValue && request.DepartmentId != Guid.Empty)
            employee.UpdateDepartment(request.DepartmentId.Value);

        if (request.BaseSalary.HasValue && request.BaseSalary > 0)
            employee.UpdateSalary(SalaryInfo.Create(request.BaseSalary.Value, "TRY"));

        if ((request.StartTime.HasValue || request.EndTime.HasValue || request.WeeklyHours.HasValue) &&
            request.StartTime.HasValue && request.EndTime.HasValue && request.WeeklyHours.HasValue)
            employee.UpdateWorkingHours(WorkingHours.Create(request.StartTime.Value, request.EndTime.Value, request.WeeklyHours.Value));

        if (!string.IsNullOrEmpty(request.Notes))
            employee.UpdateNotes(request.Notes);

        await _employeeRepository.UpdateAsync(employee, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        var dto = _mapper.Map<EmployeeDto>(employee);
        return Result<EmployeeDto>.Success(dto);
    }
}