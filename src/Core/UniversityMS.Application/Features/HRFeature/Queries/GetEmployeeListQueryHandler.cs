using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using UniversityMS.Application.Common.Models;
using UniversityMS.Application.Features.HRFeature.DTOs;
using UniversityMS.Domain.Entities.HRAggregate;
using UniversityMS.Domain.Interfaces;
using UniversityMS.Domain.Specifications;

namespace UniversityMS.Application.Features.HRFeature.Queries;

public class GetEmployeeListQueryHandler : IRequestHandler<GetEmployeeListQuery, Result<PaginatedList<EmployeeDto>>>
{
    private readonly IRepository<Employee> _repository;
    private readonly IMapper _mapper;
    private readonly ILogger<GetEmployeeListQueryHandler> _logger;

    public GetEmployeeListQueryHandler(
        IRepository<Employee> repository,
        IMapper mapper,
        ILogger<GetEmployeeListQueryHandler> logger)
    {
        _repository = repository;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<Result<PaginatedList<EmployeeDto>>> Handle(
        GetEmployeeListQuery request,
        CancellationToken cancellationToken)
    {
        try
        {
            var spec = new EmployeeFilteredSpecification(
                request.DepartmentId,
                request.Status,
                request.SearchTerm,
                request.PageNumber,
                request.PageSize);

            var employees = await _repository.ListAsync(spec, cancellationToken);
            var total = await _repository.CountAsync(spec, cancellationToken);
            var dtos = _mapper.Map<List<EmployeeDto>>(employees);

            var paginated = new PaginatedList<EmployeeDto>(
                dtos, total, request.PageNumber, request.PageSize);

            _logger.LogInformation("Retrieved {Count} employees", employees.Count);
            return Result<PaginatedList<EmployeeDto>>.Success(paginated);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving employees");
            return Result<PaginatedList<EmployeeDto>>.Failure(
                $"Çalışanlar alınırken hata oluştu: {ex.Message}");
        }
    }
}