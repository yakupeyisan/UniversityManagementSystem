using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using UniversityMS.Application.Common.Models;
using UniversityMS.Application.Features.HRFeature.DTOs;
using UniversityMS.Domain.Entities.HRAggregate;
using UniversityMS.Domain.Interfaces;

namespace UniversityMS.Application.Features.HRFeature.Queries;

/// <summary>
/// Eğitimleri listele Handler
/// </summary>
public class GetTrainingListQueryHandler : IRequestHandler<GetTrainingListQuery, Result<PaginatedList<TrainingDto>>>
{
    private readonly IRepository<Training> _trainingRepository;
    private readonly IMapper _mapper;
    private readonly ILogger<GetTrainingListQueryHandler> _logger;

    public GetTrainingListQueryHandler(
        IRepository<Training> trainingRepository,
        IMapper mapper,
        ILogger<GetTrainingListQueryHandler> logger)
    {
        _trainingRepository = trainingRepository;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<Result<PaginatedList<TrainingDto>>> Handle(
        GetTrainingListQuery request,
        CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation(
                "Eğitim listesi getiriliyor. Sayfa: {Page}, Boyut: {Size}",
                request.PageNumber, request.PageSize);

            // ========== TÜM EĞİTİMLERİ ALMA ==========
            var allTrainings = await _trainingRepository.GetAllAsync(cancellationToken);

            // ========== FİLTRELEME ==========
            var filtered = allTrainings.AsEnumerable();

            // FIX: Training.Participants yerine IsEmployeeEnrolled() metodu kullan
            if (request.EmployeeId.HasValue && request.EmployeeId != Guid.Empty)
                filtered = filtered.Where(t => t.IsEmployeeEnrolled(request.EmployeeId.Value));

            if (!string.IsNullOrEmpty(request.Status))
                filtered = filtered.Where(t => t.Status.ToString() == request.Status);

            if (request.FromDate.HasValue)
                filtered = filtered.Where(t => t.StartDate >= request.FromDate.Value);

            if (request.ToDate.HasValue)
                filtered = filtered.Where(t => t.EndDate <= request.ToDate.Value);

            // ========== SAYFALAMA ==========
            var totalCount = filtered.Count();
            var pagedTrainings = filtered
                .OrderByDescending(t => t.StartDate)
                .Skip((request.PageNumber - 1) * request.PageSize)
                .Take(request.PageSize)
                .ToList();

            // ========== DTO MAPPING ==========
            var dtos = _mapper.Map<List<TrainingDto>>(pagedTrainings);

            // ========== PAGINATED RESULT ==========
            var paginatedResult = new PaginatedList<TrainingDto>(
                dtos,
                totalCount,
                request.PageNumber,
                request.PageSize);

            _logger.LogInformation(
                "Eğitim listesi başarıyla getirілді. Toplam: {Total}, Gösterilen: {Shown}",
                totalCount, dtos.Count);

            return Result<PaginatedList<TrainingDto>>.Success(paginatedResult);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Eğitim listesi getirme hatası");
            return Result<PaginatedList<TrainingDto>>.Failure($"Eğitim listesi getirme hatası: {ex.Message}");
        }
    }
}