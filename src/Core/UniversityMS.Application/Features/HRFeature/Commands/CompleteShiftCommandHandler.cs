using MediatR;
using Microsoft.Extensions.Logging;
using UniversityMS.Application.Common.Models;
using UniversityMS.Domain.Entities.HRAggregate;
using UniversityMS.Domain.Interfaces;

namespace UniversityMS.Application.Features.HRFeature.Commands;

public class CompleteShiftCommandHandler : IRequestHandler<CompleteShiftCommand, Result<Unit>>
{
    private readonly IRepository<Shift> _shiftRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<CompleteShiftCommandHandler> _logger;

    public CompleteShiftCommandHandler(
        IRepository<Shift> shiftRepository,
        IUnitOfWork unitOfWork,
        ILogger<CompleteShiftCommandHandler> logger)
    {
        _shiftRepository = shiftRepository;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Result<Unit>> Handle(
        CompleteShiftCommand request,
        CancellationToken cancellationToken)
    {
        try
        {
            var shift = await _shiftRepository.GetByIdAsync(request.ShiftId, cancellationToken);
            if (shift == null)
                return Result<Unit>.Failure("Vardiya bulunamadı.");

            shift.Complete(request.OvertimeHours);

            await _shiftRepository.UpdateAsync(shift, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Shift completed: {ShiftId} with {Overtime} hours overtime",
                request.ShiftId, request.OvertimeHours ?? 0);

            return Result<Unit>.Success(Unit.Value, "Vardiya başarıyla tamamlandı.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error completing shift");
            return Result<Unit>.Failure($"Vardiya tamamlanırken hata: {ex.Message}");
        }
    }
}