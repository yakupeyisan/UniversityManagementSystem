using MediatR;
using Microsoft.Extensions.Logging;
using UniversityMS.Application.Common.Models;
using UniversityMS.Domain.Entities.HRAggregate;
using UniversityMS.Domain.Interfaces;

namespace UniversityMS.Application.Features.HRFeature.Commands;

public class ApproveLeaveCommandHandler : IRequestHandler<ApproveLeaveCommand, Result<Unit>>
{
    private readonly IRepository<Leave> _leaveRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<ApproveLeaveCommandHandler> _logger;

    public ApproveLeaveCommandHandler(
        IRepository<Leave> leaveRepository,
        IUnitOfWork unitOfWork,
        ILogger<ApproveLeaveCommandHandler> logger)
    {
        _leaveRepository = leaveRepository;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Result<Unit>> Handle(
        ApproveLeaveCommand request,
        CancellationToken cancellationToken)
    {
        try
        {
            var leave = await _leaveRepository.GetByIdAsync(request.LeaveId, cancellationToken);
            if (leave == null)
                return Result<Unit>.Failure("İzin bulunamadı.");

            leave.Approve(request.ApproverId);

            await _leaveRepository.UpdateAsync(leave, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Leave approved: {LeaveId} by {ApproverId}",
                request.LeaveId, request.ApproverId);

            return Result<Unit>.Success(Unit.Value, "İzin başarıyla onaylandı.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error approving leave");
            return Result<Unit>.Failure($"İzin onaylanırken hata: {ex.Message}");
        }
    }
}