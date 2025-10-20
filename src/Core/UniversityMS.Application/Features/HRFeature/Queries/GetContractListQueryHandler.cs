using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using UniversityMS.Application.Common.Models;
using UniversityMS.Application.Features.HRFeature.DTOs;
using UniversityMS.Domain.Entities.HRAggregate;
using UniversityMS.Domain.Interfaces;
using UniversityMS.Domain.Specifications;

namespace UniversityMS.Application.Features.HRFeature.Queries;

public class GetContractListQueryHandler : IRequestHandler<GetContractListQuery, Result<PaginatedList<ContractDto>>>
{
    private readonly IRepository<Contract> _repository;
    private readonly IMapper _mapper;
    private readonly ILogger<GetContractListQueryHandler> _logger;

    public GetContractListQueryHandler(
        IRepository<Contract> repository,
        IMapper mapper,
        ILogger<GetContractListQueryHandler> logger)
    {
        _repository = repository;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<Result<PaginatedList<ContractDto>>> Handle(
        GetContractListQuery request,
        CancellationToken cancellationToken)
    {
        try
        {
            var spec = new ContractFilteredSpecification(
                request.EmployeeId,
                request.Status,
                request.ContractType,
                request.PageNumber,
                request.PageSize);

            var contracts = await _repository.ListAsync(spec, cancellationToken);
            var total = await _repository.CountAsync(spec, cancellationToken);
            var dtos = _mapper.Map<List<ContractDto>>(contracts);

            var paginated = new PaginatedList<ContractDto>(
                dtos, total, request.PageNumber, request.PageSize);

            _logger.LogInformation("Retrieved {Count} contracts", contracts.Count);
            return Result<PaginatedList<ContractDto>>.Success(paginated);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving contracts");
            return Result<PaginatedList<ContractDto>>.Failure(
                $"Sözleşmeler alınırken hata oluştu: {ex.Message}");
        }
    }
}