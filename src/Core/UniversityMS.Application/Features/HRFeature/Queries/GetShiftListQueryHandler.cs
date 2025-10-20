using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using UniversityMS.Application.Common.Models;
using UniversityMS.Application.Features.HRFeature.DTOs;
using UniversityMS.Domain.Entities.HRAggregate;
using UniversityMS.Domain.Interfaces;
using UniversityMS.Domain.Specifications;

namespace UniversityMS.Application.Features.HRFeature.Queries;

public class GetShiftListQueryHandler : IRequestHandler<GetShiftListQuery, Result<PaginatedList<ShiftDto>>>
{
    private readonly IRepository<Shift> _repository;
    private readonly IMapper _mapper;
    private readonly ILogger<GetShiftListQueryHandler> _logger;

    public GetShiftListQueryHandler(
        IRepository<Shift> repository,
        IMapper mapper,
        ILogger<GetShiftListQueryHandler> logger)
    {
        _repository = repository;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<Result<PaginatedList<ShiftDto>>> Handle(
        GetShiftListQuery request,
        CancellationToken cancellationToken)
    {
        try
        {
            var spec = new ShiftFilteredSpecification(
                request.EmployeeId,
                request.Status,
                request.FromDate,
                request.ToDate,
                request.PageNumber,
                request.PageSize);

            var shifts = await _repository.ListAsync(spec, cancellationToken);
            var total = await _repository.CountAsync(spec, cancellationToken);
            var dtos = _mapper.Map<List<ShiftDto>>(shifts);

            var paginated = new PaginatedList<ShiftDto>(
                dtos, total, request.PageNumber, request.PageSize);

            _logger.LogInformation("Retrieved {Count} shifts", shifts.Count);
            return Result<PaginatedList<ShiftDto>>.Success(paginated);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving shifts");
            return Result<PaginatedList<ShiftDto>>.Failure(
                $"Vardiyalar alınırken hata oluştu: {ex.Message}");
        }
    }
}