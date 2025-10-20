using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using UniversityMS.Application.Common.Models;
using UniversityMS.Application.Features.FinanceFeature.DTOs;
using UniversityMS.Domain.Entities.FinanceAggregate;
using UniversityMS.Domain.Interfaces;
using UniversityMS.Domain.Specifications;

namespace UniversityMS.Application.Features.FinanceFeature.Queries;

public class GetInvoiceListQueryHandler : IRequestHandler<GetInvoiceListQuery, Result<PaginatedList<InvoiceDto>>>
{
    private readonly IRepository<Invoice> _repository;
    private readonly IMapper _mapper;
    private readonly ILogger<GetInvoiceListQueryHandler> _logger;

    public GetInvoiceListQueryHandler(
        IRepository<Invoice> repository,
        IMapper mapper,
        ILogger<GetInvoiceListQueryHandler> logger)
    {
        _repository = repository;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<Result<PaginatedList<InvoiceDto>>> Handle(
        GetInvoiceListQuery request,
        CancellationToken cancellationToken)
    {
        try
        {
            var spec = new InvoiceFilteredSpecification(
                request.Status,
                request.Type,
                request.FromDate,
                request.ToDate,
                request.SupplierId,
                request.PageNumber,
                request.PageSize);

            var invoices = await _repository.ListAsync(spec, cancellationToken);
            var total = await _repository.CountAsync(spec, cancellationToken);
            var dtos = _mapper.Map<List<InvoiceDto>>(invoices);

            var paginated = new PaginatedList<InvoiceDto>(
                dtos, total, request.PageNumber, request.PageSize);

            _logger.LogInformation("Retrieved {Count} invoices", invoices.Count);
            return Result<PaginatedList<InvoiceDto>>.Success(paginated);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving invoices");
            return Result<PaginatedList<InvoiceDto>>.Failure(
                $"Faturalar alınırken hata oluştu: {ex.Message}");
        }
    }
}