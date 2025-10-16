using MediatR;
using UniversityMS.Application.Common.Models;
using UniversityMS.Domain.Entities.HRAggregate;
using UniversityMS.Domain.Interfaces;

namespace UniversityMS.Application.Features.HR.Commands;

public class TerminateEmployeeCommandHandler : IRequestHandler<TerminateEmployeeCommand, Result<Unit>>
{
    private readonly IRepository<Employee> _employeeRepository;
    private readonly IUnitOfWork _unitOfWork;

    public TerminateEmployeeCommandHandler(
        IRepository<Employee> employeeRepository,
        IUnitOfWork unitOfWork)
    {
        _employeeRepository = employeeRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<Unit>> Handle(
        TerminateEmployeeCommand request,
        CancellationToken cancellationToken)
    {
        var employee = await _employeeRepository.GetByIdAsync(request.EmployeeId, cancellationToken);
        if (employee is null)
            return Result.Failure<Unit>("Çalışan bulunamadı");

        if (request.TerminationDate < employee.HireDate)
            return Result.Failure<Unit>("İşten ayrılış tarihi işe alım tarihinden önce olamaz");

        employee.Terminate(request.TerminationDate);

        await _employeeRepository.UpdateAsync(employee, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result<Unit>.Success(Unit.Value);
    }
}