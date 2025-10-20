using MediatR;
using Microsoft.Extensions.Logging;
using UniversityMS.Application.Common.Interfaces;
using UniversityMS.Application.Common.Models;
using UniversityMS.Domain.Entities.ProcurementAggregate;
using UniversityMS.Domain.Interfaces;

namespace UniversityMS.Application.Features.ProcurementFeature.Commands;

public class ApprovePurchaseRequestCommandHandler : IRequestHandler<ApprovePurchaseRequestCommand, Result<Guid>>
{
    private readonly IRepository<PurchaseRequest> _repository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUserService;
    private readonly ILogger<ApprovePurchaseRequestCommandHandler> _logger;

    public ApprovePurchaseRequestCommandHandler(
        IRepository<PurchaseRequest> repository,
        IUnitOfWork unitOfWork,
        ICurrentUserService currentUserService,
        ILogger<ApprovePurchaseRequestCommandHandler> logger)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
        _currentUserService = currentUserService;
        _logger = logger;
    }

    public async Task<Result<Guid>> Handle(ApprovePurchaseRequestCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var pr = await _repository.GetByIdAsync(request.RequestId, cancellationToken);
            if (pr == null)
                return Result<Guid>.Failure("Satın alma talebi bulunamadı.");

            pr.Approve(_currentUserService.UserId);
            await _repository.UpdateAsync(pr, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Purchase request approved: {RequestId}", request.RequestId);
            return Result<Guid>.Success(pr.Id, "Talep onaylandı.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error approving purchase request");
            return Result<Guid>.Failure($"Hata: {ex.Message}");
        }
    }
}