using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using UniversityMS.Application.Common.Models;
using UniversityMS.Application.Features.HRFeature.DTOs;
using UniversityMS.Domain.Entities.HRAggregate;
using UniversityMS.Domain.Interfaces;

namespace UniversityMS.Application.Features.HRFeature.Queries;

/// <summary>
/// İzin talebini ID'ye göre getir Handler
/// </summary>
public class GetLeaveByIdQueryHandler : IRequestHandler<GetLeaveByIdQuery, Result<LeaveDetailDto>>
{
    private readonly IRepository<Leave> _leaveRepository;
    private readonly IMapper _mapper;
    private readonly ILogger<GetLeaveByIdQueryHandler> _logger;

    public GetLeaveByIdQueryHandler(
        IRepository<Leave> leaveRepository,
        IMapper mapper,
        ILogger<GetLeaveByIdQueryHandler> logger)
    {
        _leaveRepository = leaveRepository;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<Result<LeaveDetailDto>> Handle(
        GetLeaveByIdQuery request,
        CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("İzin detayı getiriliyor. LeaveId: {LeaveId}", request.LeaveId);

            var leave = await _leaveRepository.GetByIdAsync(request.LeaveId, cancellationToken);

            if (leave == null)
            {
                _logger.LogWarning("İzin bulunamadı. LeaveId: {LeaveId}", request.LeaveId);
                return Result<LeaveDetailDto>.Failure("İzin talebi bulunamadı.");
            }

            var dto = _mapper.Map<LeaveDetailDto>(leave);

            _logger.LogInformation(
                "İzin detayı başarıyla getirілді. LeaveId: {LeaveId}, Status: {Status}",
                request.LeaveId, leave.Status);

            return Result<LeaveDetailDto>.Success(dto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "İzin detayı getirme hatası. LeaveId: {LeaveId}", request.LeaveId);
            return Result<LeaveDetailDto>.Failure($"İzin detayı getirme hatası: {ex.Message}");
        }
    }
}
