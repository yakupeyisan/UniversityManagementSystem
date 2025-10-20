using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using UniversityMS.Application.Common.Models;
using UniversityMS.Application.Features.StaffFeature.DTOs;
using UniversityMS.Domain.Entities.HRAggregate;
using UniversityMS.Domain.Interfaces;
using UniversityMS.Domain.Specifications;

namespace UniversityMS.Application.Features.HRFeature.Queries;

public class GetLeaveListQueryHandler : IRequestHandler<GetLeaveListQuery, Result<PaginatedList<LeaveRequestDto>>>
{
    private readonly IRepository<Leave> _repository;
    private readonly IMapper _mapper;
    private readonly ILogger<GetLeaveListQueryHandler> _logger;

    public GetLeaveListQueryHandler(
        IRepository<Leave> repository,
        IMapper mapper,
        ILogger<GetLeaveListQueryHandler> logger)
    {
        _repository = repository;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<Result<PaginatedList<LeaveRequestDto>>> Handle(
        GetLeaveListQuery request,
        CancellationToken cancellationToken)
    {
        try
        {
            var spec = new LeaveFilteredSpecification(
                request.EmployeeId,
                request.Status,
                request.LeaveType,
                request.FromDate,
                request.ToDate,
                request.PageNumber,
                request.PageSize);

            var leaves = await _repository.ListAsync(spec, cancellationToken);
            var total = await _repository.CountAsync(spec, cancellationToken);
            var dtos = _mapper.Map<List<LeaveRequestDto>>(leaves);

            var paginated = new PaginatedList<LeaveRequestDto>(
                dtos, total, request.PageNumber, request.PageSize);

            _logger.LogInformation("Retrieved {Count} leave requests", leaves.Count);
            return Result<PaginatedList<LeaveRequestDto>>.Success(paginated);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving leave requests");
            return Result<PaginatedList<LeaveRequestDto>>.Failure(
                $"İzin talepleri alınırken hata oluştu: {ex.Message}");
        }
    }
}