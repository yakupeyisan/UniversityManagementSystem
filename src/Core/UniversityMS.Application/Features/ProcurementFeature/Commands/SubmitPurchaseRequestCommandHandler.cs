using MediatR;
using Microsoft.Extensions.Logging;
using UniversityMS.Application.Common.Models;
using UniversityMS.Domain.Entities.ProcurementAggregate;
using UniversityMS.Domain.Interfaces;

namespace UniversityMS.Application.Features.ProcurementFeature.Commands;

public class SubmitPurchaseRequestCommandHandler : IRequestHandler<SubmitPurchaseRequestCommand, Result<Guid>>
{
    private readonly IRepository<PurchaseRequest> _repository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<SubmitPurchaseRequestCommandHandler> _logger;

    public SubmitPurchaseRequestCommandHandler(
        IRepository<PurchaseRequest> repository,
        IUnitOfWork unitOfWork,
        ILogger<SubmitPurchaseRequestCommandHandler> logger)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Result<Guid>> Handle(SubmitPurchaseRequestCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var pr = await _repository.GetByIdAsync(request.RequestId, cancellationToken);
            if (pr == null)
                return Result<Guid>.Failure("Satın alma talebi bulunamadı.");

            pr.Submit();
            await _repository.UpdateAsync(pr, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Purchase request submitted: {RequestId}", request.RequestId);
            return Result<Guid>.Success(pr.Id, "Talep gönderildi.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error submitting purchase request");
            return Result<Guid>.Failure($"Hata: {ex.Message}");
        }
    }
}