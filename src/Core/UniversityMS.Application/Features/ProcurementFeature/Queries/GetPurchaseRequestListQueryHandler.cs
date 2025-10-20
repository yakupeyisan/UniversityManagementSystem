using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using UniversityMS.Application.Common.Models;
using UniversityMS.Application.Features.ProcurementFeature.DTOs;
using UniversityMS.Domain.Entities.ProcurementAggregate;
using UniversityMS.Domain.Interfaces;
using UniversityMS.Domain.Specifications;

namespace UniversityMS.Application.Features.ProcurementFeature.Queries;

public class GetPurchaseRequestListQueryHandler : IRequestHandler<GetPurchaseRequestListQuery, Result<PaginatedList<PurchaseRequestDto>>>
{
    private readonly IRepository<PurchaseRequest> _repository;
    private readonly IMapper _mapper;
    private readonly ILogger<GetPurchaseRequestListQueryHandler> _logger;

    public GetPurchaseRequestListQueryHandler(
        IRepository<PurchaseRequest> repository,
        IMapper mapper,
        ILogger<GetPurchaseRequestListQueryHandler> logger)
    {
        _repository = repository;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<Result<PaginatedList<PurchaseRequestDto>>> Handle(GetPurchaseRequestListQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var spec = new PurchaseRequestFilteredSpecification(
                request.Status, request.DepartmentId, request.PageNumber, request.PageSize);

            var requests = await _repository.ListAsync(spec, cancellationToken);
            var total = await _repository.CountAsync(spec, cancellationToken);
            var dtos = _mapper.Map<List<PurchaseRequestDto>>(requests);

            var paginated = new PaginatedList<PurchaseRequestDto>(dtos, total, request.PageNumber, request.PageSize);
            return Result<PaginatedList<PurchaseRequestDto>>.Success(paginated);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving purchase requests");
            return Result<PaginatedList<PurchaseRequestDto>>.Failure($"Hata: {ex.Message}");
        }
    }
}