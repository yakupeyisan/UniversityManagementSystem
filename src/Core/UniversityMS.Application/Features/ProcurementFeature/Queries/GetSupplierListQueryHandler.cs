using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using UniversityMS.Application.Common.Models;
using UniversityMS.Application.Features.ProcurementFeature.DTOs;
using UniversityMS.Domain.Entities.ProcurementAggregate;
using UniversityMS.Domain.Interfaces;
using UniversityMS.Domain.Specifications;

namespace UniversityMS.Application.Features.ProcurementFeature.Queries;

public class GetSupplierListQueryHandler : IRequestHandler<GetSupplierListQuery, Result<PaginatedList<SupplierDto>>>
{
    private readonly IRepository<Supplier> _repository;
    private readonly IMapper _mapper;
    private readonly ILogger<GetSupplierListQueryHandler> _logger;

    public GetSupplierListQueryHandler(
        IRepository<Supplier> repository,
        IMapper mapper,
        ILogger<GetSupplierListQueryHandler> logger)
    {
        _repository = repository;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<Result<PaginatedList<SupplierDto>>> Handle(
        GetSupplierListQuery request,
        CancellationToken cancellationToken)
    {
        try
        {
            var spec = new SupplierFilteredSpecification(
                request.Status,
                request.SearchTerm,
                request.PageNumber,
                request.PageSize);

            var suppliers = await _repository.ListAsync(spec, cancellationToken);
            var total = await _repository.CountAsync(spec, cancellationToken);
            var dtos = _mapper.Map<List<SupplierDto>>(suppliers);

            var paginated = new PaginatedList<SupplierDto>(
                dtos, total, request.PageNumber, request.PageSize);

            _logger.LogInformation("Retrieved {Count} suppliers", suppliers.Count);
            return Result<PaginatedList<SupplierDto>>.Success(paginated);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving suppliers");
            return Result<PaginatedList<SupplierDto>>.Failure(
                $"Tedarikçiler alınırken hata oluştu: {ex.Message}");
        }
    }
}