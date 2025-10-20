using AutoMapper;
using MediatR;
using UniversityMS.Application.Common.Models;
using UniversityMS.Application.Features.ProcurementFeature.DTOs;
using UniversityMS.Domain.Entities.ProcurementAggregate;
using UniversityMS.Domain.Interfaces;
using UniversityMS.Domain.Specifications;

namespace UniversityMS.Application.Features.ProcurementFeature.Queries;

public class GetPurchaseOrderListQueryHandler
    : IRequestHandler<GetPurchaseOrderListQuery, Result<PaginatedList<PurchaseOrderDto>>>
{
    private readonly IRepository<PurchaseOrder> _repository;
    private readonly IMapper _mapper;

    public GetPurchaseOrderListQueryHandler(
        IRepository<PurchaseOrder> repository,
        IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public async Task<Result<PaginatedList<PurchaseOrderDto>>> Handle(
        GetPurchaseOrderListQuery request,
        CancellationToken cancellationToken)
    {
        try
        {
            // Specification ile filtreleme
            var specification = new PurchaseOrderFilteredSpecification(
                status: request.Status,
                supplierId: request.SupplierId,
                pageNumber: request.PageNumber,
                pageSize: request.PageSize);

            var orders = await _repository.ListAsync(specification, cancellationToken);
            var total = await _repository.CountAsync(specification, cancellationToken);

            var dtos = _mapper.Map<List<PurchaseOrderDto>>(orders);
            var paginated = new PaginatedList<PurchaseOrderDto>(
                dtos, total, request.PageNumber, request.PageSize);

            return Result<PaginatedList<PurchaseOrderDto>>.Success(paginated);
        }
        catch (Exception ex)
        {
            return Result<PaginatedList<PurchaseOrderDto>>.Failure($"Hata: {ex.Message}");
        }
    }
}