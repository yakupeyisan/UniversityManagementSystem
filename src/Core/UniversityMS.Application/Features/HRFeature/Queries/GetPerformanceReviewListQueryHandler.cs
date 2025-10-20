using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using UniversityMS.Application.Common.Models;
using UniversityMS.Application.Features.HRFeature.DTOs;
using UniversityMS.Domain.Entities.HRAggregate;
using UniversityMS.Domain.Interfaces;

namespace UniversityMS.Application.Features.HRFeature.Queries;

/// <summary>
/// Performans değerlendirmelerini listele Handler
/// </summary>
public class GetPerformanceReviewListQueryHandler : IRequestHandler<GetPerformanceReviewListQuery, Result<PaginatedList<PerformanceReviewDto>>>
{
    private readonly IRepository<PerformanceReview> _performanceReviewRepository;
    private readonly IMapper _mapper;
    private readonly ILogger<GetPerformanceReviewListQueryHandler> _logger;

    public GetPerformanceReviewListQueryHandler(
        IRepository<PerformanceReview> performanceReviewRepository,
        IMapper mapper,
        ILogger<GetPerformanceReviewListQueryHandler> logger)
    {
        _performanceReviewRepository = performanceReviewRepository;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<Result<PaginatedList<PerformanceReviewDto>>> Handle(
        GetPerformanceReviewListQuery request,
        CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation(
                "Performans değerlendirmesi listesi getiriliyor. Sayfa: {Page}, Boyut: {Size}",
                request.PageNumber, request.PageSize);

            // ========== FİLTRELENMİŞ LISTESI ALMA ==========
            var allReviews = await _performanceReviewRepository.GetAllAsync(cancellationToken);

            var filtered = allReviews.AsEnumerable();

            if (request.EmployeeId.HasValue && request.EmployeeId != Guid.Empty)
                filtered = filtered.Where(pr => pr.EmployeeId == request.EmployeeId);

            if (!string.IsNullOrEmpty(request.Status))
                filtered = filtered.Where(pr => pr.Status.ToString() == request.Status);

            if (request.Year.HasValue)
                filtered = filtered.Where(pr => pr.ReviewDate.Year == request.Year);

            // ========== SAYFALAMA ==========
            var totalCount = filtered.Count();
            var pagedReviews = filtered
                .OrderByDescending(pr => pr.ReviewDate)
                .Skip((request.PageNumber - 1) * request.PageSize)
                .Take(request.PageSize)
                .ToList();

            // ========== DTO MAPPING ==========
            var dtos = _mapper.Map<List<PerformanceReviewDto>>(pagedReviews);

            // ========== PAGINATED RESULT ==========
            var paginatedResult = new PaginatedList<PerformanceReviewDto>(
                dtos,
                totalCount,
                request.PageNumber,
                request.PageSize);

            _logger.LogInformation(
                "Performans değerlendirmesi listesi başarıyla getirілді. Toplam: {Total}",
                totalCount);

            return Result<PaginatedList<PerformanceReviewDto>>.Success(paginatedResult);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Performans değerlendirmesi listesi getirme hatası");
            return Result<PaginatedList<PerformanceReviewDto>>.Failure($"Hata: {ex.Message}");
        }
    }
}