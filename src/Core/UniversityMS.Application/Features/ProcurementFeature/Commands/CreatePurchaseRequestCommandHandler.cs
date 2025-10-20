using MediatR;
using Microsoft.Extensions.Logging;
using UniversityMS.Application.Common.Interfaces;
using UniversityMS.Application.Common.Models;
using UniversityMS.Domain.Entities.ProcurementAggregate;
using UniversityMS.Domain.Interfaces;

namespace UniversityMS.Application.Features.ProcurementFeature.Commands;

public class CreatePurchaseRequestCommandHandler : IRequestHandler<CreatePurchaseRequestCommand, Result<Guid>>
{
    private readonly IRepository<PurchaseRequest> _repository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUserService;
    private readonly ILogger<CreatePurchaseRequestCommandHandler> _logger;

    public CreatePurchaseRequestCommandHandler(
        IRepository<PurchaseRequest> repository,
        IUnitOfWork unitOfWork,
        ICurrentUserService currentUserService,
        ILogger<CreatePurchaseRequestCommandHandler> logger)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
        _currentUserService = currentUserService;
        _logger = logger;
    }

    public async Task<Result<Guid>> Handle(CreatePurchaseRequestCommand request, CancellationToken cancellationToken)
    {
        try
        {
            // Generate request number: PR-2024001
            var requestNumber = $"PR-{DateTime.UtcNow:yyyy}{DateTime.UtcNow.DayOfYear:D5}";

            var purchaseRequest = PurchaseRequest.Create(
                requestNumber,
                request.DepartmentId,
                request.Title,
                request.Description,
                request.RequiredDate,
                request.Priority,
                _currentUserService.UserId
            );

            await _repository.AddAsync(purchaseRequest, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Purchase request created: {RequestNumber} by {UserId}",
                requestNumber, _currentUserService.UserId);

            return Result<Guid>.Success(purchaseRequest.Id, "Satın alma talebi oluşturuldu.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating purchase request");
            return Result<Guid>.Failure($"Hata: {ex.Message}");
        }
    }
}
