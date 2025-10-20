using MediatR;
using Microsoft.Extensions.Logging;
using UniversityMS.Application.Common.Models;
using UniversityMS.Domain.Entities.HRAggregate;
using UniversityMS.Domain.Enums;
using UniversityMS.Domain.Interfaces;

namespace UniversityMS.Application.Features.HRFeature.Commands;

public class AssignShiftCommandHandler : IRequestHandler<AssignShiftCommand, Result<Guid>>
{
    private readonly IRepository<Shift> _shiftRepository;
    private readonly IRepository<Employee> _employeeRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<AssignShiftCommandHandler> _logger;

    public AssignShiftCommandHandler(
        IRepository<Shift> shiftRepository,
        IRepository<Employee> employeeRepository,
        IUnitOfWork unitOfWork,
        ILogger<AssignShiftCommandHandler> logger)
    {
        _shiftRepository = shiftRepository;
        _employeeRepository = employeeRepository;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Result<Guid>> Handle(
        AssignShiftCommand request,
        CancellationToken cancellationToken)
    {
        try
        {
            var employee = await _employeeRepository.GetByIdAsync(request.EmployeeId, cancellationToken);
            if (employee == null)
                return Result<Guid>.Failure("Çalışan bulunamadı.");

            var shiftPattern = Enum.Parse<ShiftPattern>(request.ShiftPattern);
            var shift = Shift.Create(
                request.EmployeeId,
                request.Date,
                request.StartTime,
                request.EndTime,
                shiftPattern,
                request.Notes);

            employee.AssignShift(shift);

            await _shiftRepository.AddAsync(shift, cancellationToken);
            await _employeeRepository.UpdateAsync(employee, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Shift assigned: {EmployeeId} on {Date}",
                request.EmployeeId, request.Date);

            return Result<Guid>.Success(shift.Id, "Vardiya başarıyla atandı.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error assigning shift");
            return Result<Guid>.Failure($"Vardiya atanırken hata: {ex.Message}");
        }
    }
}