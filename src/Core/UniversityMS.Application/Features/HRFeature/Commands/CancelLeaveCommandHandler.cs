using MediatR;
using Microsoft.Extensions.Logging;
using UniversityMS.Application.Common.Models;
using UniversityMS.Domain.Entities.HRAggregate;
using UniversityMS.Domain.Interfaces;

namespace UniversityMS.Application.Features.HRFeature.Commands;

public class CancelLeaveCommandHandler : IRequestHandler<CancelLeaveCommand, Result<Unit>>
{
    private readonly IRepository<Leave> _leaveRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<CancelLeaveCommandHandler> _logger;

    public CancelLeaveCommandHandler(
        IRepository<Leave> leaveRepository,
        IUnitOfWork unitOfWork,
        ILogger<CancelLeaveCommandHandler> logger)
    {
        _leaveRepository = leaveRepository;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Result<Unit>> Handle(
        CancelLeaveCommand request,
        CancellationToken cancellationToken)
    {
        try
        {
            var leave = await _leaveRepository.GetByIdAsync(request.LeaveId, cancellationToken);
            if (leave == null)
                return Result<Unit>.Failure("İzin bulunamadı.");

            leave.Cancel();

            await _leaveRepository.UpdateAsync(leave, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Leave cancelled: {LeaveId}", request.LeaveId);
            return Result<Unit>.Success(Unit.Value, "İzin başarıyla iptal edildi.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error cancelling leave");
            return Result<Unit>.Failure($"İzin iptal edilirken hata: {ex.Message}");
        }
    }
}