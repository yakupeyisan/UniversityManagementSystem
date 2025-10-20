using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using UniversityMS.Application.Common.Models;
using UniversityMS.Application.Features.HRFeature.DTOs;
using UniversityMS.Domain.Entities.HRAggregate;
using UniversityMS.Domain.Interfaces;

namespace UniversityMS.Application.Features.HRFeature.Queries;

public class GetLeaveByIdQueryHandler : IRequestHandler<GetLeaveByIdQuery, Result<LeaveDetailDto>>
{
    private readonly IRepository<Leave> _repository;
    private readonly IMapper _mapper;
    private readonly ILogger<GetLeaveByIdQueryHandler> _logger;

    public GetLeaveByIdQueryHandler(
        IRepository<Leave> repository,
        IMapper mapper,
        ILogger<GetLeaveByIdQueryHandler> logger)
    {
        _repository = repository;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<Result<LeaveDetailDto>> Handle(
        GetLeaveByIdQuery request,
        CancellationToken cancellationToken)
    {
        try
        {
            if (request.LeaveId == Guid.Empty)
                return Result<LeaveDetailDto>.Failure("Geçersiz izin ID'si.");

            var leave = await _repository.GetByIdAsync(request.LeaveId, cancellationToken);
            if (leave == null)
                return Result<LeaveDetailDto>.Failure("İzin bulunamadı.");

            var dto = _mapper.Map<LeaveDetailDto>(leave);
            _logger.LogInformation("Retrieved leave: {LeaveId}", request.LeaveId);
            return Result<LeaveDetailDto>.Success(dto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving leave {LeaveId}", request.LeaveId);
            return Result<LeaveDetailDto>.Failure(
                $"İzin alınırken hata oluştu: {ex.Message}");
        }
    }
}