using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using UniversityMS.Application.Common.Models;
using UniversityMS.Application.Features.HRFeature.DTOs;
using UniversityMS.Domain.Entities.HRAggregate;
using UniversityMS.Domain.Interfaces;
using UniversityMS.Domain.Specifications;

namespace UniversityMS.Application.Features.HRFeature.Queries;

/// <summary>
/// Vardiyaları listele Handler
/// </summary>
public class GetShiftListQueryHandler : IRequestHandler<GetShiftListQuery, Result<PaginatedList<ShiftDto>>>
{
    private readonly IRepository<Shift> _shiftRepository;
    private readonly IMapper _mapper;
    private readonly ILogger<GetShiftListQueryHandler> _logger;

    public GetShiftListQueryHandler(
        IRepository<Shift> shiftRepository,
        IMapper mapper,
        ILogger<GetShiftListQueryHandler> logger)
    {
        _shiftRepository = shiftRepository;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<Result<PaginatedList<ShiftDto>>> Handle(
        GetShiftListQuery request,
        CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation(
                "Vardiya listesi getiriliyor. Sayfa: {Page}, Boyut: {Size}",
                request.PageNumber, request.PageSize);

            // ========== SPECIFICATION OLUŞTURMA ==========
            var spec = new ShiftFilteredSpecification(
                employeeId: request.EmployeeId,
                status: request.Status,
                fromDate: request.FromDate,
                toDate: request.ToDate,
                pageNumber: request.PageNumber,
                pageSize: request.PageSize);

            // ========== VARDIYALARI GETIRME ==========
            var shifts = await _shiftRepository.ListAsync(spec, cancellationToken);
            var totalCount = await _shiftRepository.CountAsync(spec, cancellationToken);

            // ========== DTO MAPPING ==========
            var dtos = _mapper.Map<List<ShiftDto>>(shifts);

            // ========== PAGINATED RESULT ==========
            var paginatedResult = new PaginatedList<ShiftDto>(
                dtos,
                totalCount,
                request.PageNumber,
                request.PageSize);

            _logger.LogInformation(
                "Vardiya listesi başarıyla getirілді. Toplam: {Total}, Gösterilen: {Shown}",
                totalCount, dtos.Count);

            return Result<PaginatedList<ShiftDto>>.Success(paginatedResult);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Vardiya listesi getirme hatası");
            return Result<PaginatedList<ShiftDto>>.Failure($"Vardiya listesi getirme hatası: {ex.Message}");
        }
    }
}

