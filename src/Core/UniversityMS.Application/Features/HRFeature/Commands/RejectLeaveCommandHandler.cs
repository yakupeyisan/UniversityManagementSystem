using MediatR;
using Microsoft.Extensions.Logging;
using UniversityMS.Application.Common.Models;
using UniversityMS.Domain.Entities.HRAggregate;
using UniversityMS.Domain.Interfaces;

namespace UniversityMS.Application.Features.HRFeature.Commands;

public class RejectLeaveCommandHandler : IRequestHandler<RejectLeaveCommand, Result<Unit>>
{
    private readonly IRepository<Leave> _leaveRepository;
    private readonly IRepository<Employee> _employeeRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<RejectLeaveCommandHandler> _logger;

    public RejectLeaveCommandHandler(
        IRepository<Leave> leaveRepository,
        IRepository<Employee> employeeRepository,
        IUnitOfWork unitOfWork,
        ILogger<RejectLeaveCommandHandler> logger)
    {
        _leaveRepository = leaveRepository;
        _employeeRepository = employeeRepository;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Result<Unit>> Handle(
        RejectLeaveCommand request,
        CancellationToken cancellationToken)
    {
        try
        {
            var leave = await _leaveRepository.GetByIdAsync(request.LeaveId, cancellationToken);
            if (leave == null)
                return Result<Unit>.Failure("İzin bulunamadı.");

            var employee = await _employeeRepository.GetByIdAsync(leave.EmployeeId, cancellationToken);
            if (employee == null)
                return Result<Unit>.Failure("Çalışan bulunamadı.");

            leave.Reject(request.RejectingUserId, request.Reason);
            employee.RejectLeave(request.LeaveId, request.RejectingUserId, request.Reason);

            await _leaveRepository.UpdateAsync(leave, cancellationToken);
            await _employeeRepository.UpdateAsync(employee, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Leave rejected: {LeaveId} - Reason: {Reason}",
                request.LeaveId, request.Reason);

            return Result<Unit>.Success(Unit.Value, "İzin başarıyla reddedildi.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error rejecting leave");
            return Result<Unit>.Failure($"İzin reddedilirken hata: {ex.Message}");
        }
    }
}