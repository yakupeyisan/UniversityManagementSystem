using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using System.Linq.Expressions;
using UniversityMS.Application.Common.Extensions;
using UniversityMS.Application.Common.Models;
using UniversityMS.Application.Features.DepartmentFeature.DTOs;
using UniversityMS.Domain.Entities.AcademicAggregate;
using UniversityMS.Domain.Filters;
using UniversityMS.Domain.Interfaces;
using UniversityMS.Domain.Specifications;

namespace UniversityMS.Application.Features.DepartmentFeature.Queries;
public class GetDepartmentListQueryHandler : IRequestHandler<GetDepartmentListQuery, Result<PaginatedList<DepartmentDto>>>
{
    private readonly IRepository<Department> _departmentRepository;
    private readonly IFilterParser<Department> _filterParser;
    private readonly IMapper _mapper;
    private readonly ILogger<GetDepartmentListQueryHandler> _logger;

    public GetDepartmentListQueryHandler(
        IRepository<Department> departmentRepository,
        IFilterParser<Department> filterParser,
        IMapper mapper,
        ILogger<GetDepartmentListQueryHandler> logger)
    {
        _departmentRepository = departmentRepository;
        _filterParser = filterParser;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<Result<PaginatedList<DepartmentDto>>> Handle(
        GetDepartmentListQuery request,
        CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation(
                "Department list requested. PageNumber: {PageNumber}, PageSize: {PageSize}, Filter: {Filter}",
                request.PageNumber,
                request.PageSize,
                request.Filter ?? "None");

            // ✅ SPECIFICATION PATTERN
            var specification = new DepartmentFilteredSpecification(
                filterString: request.Filter,
                pageNumber: request.PageNumber,
                pageSize: request.PageSize,
                filterParser: _filterParser);

            var departments = await _departmentRepository.ListAsync(specification, cancellationToken);
            var totalCount = await _departmentRepository.CountAsync(specification, cancellationToken);

            _logger.LogInformation(
                "Departments retrieved. TotalCount: {TotalCount}, Returned: {ReturnedCount}",
                totalCount,
                departments.Count);

            var departmentDtos = _mapper.Map<List<DepartmentDto>>(departments);
            var paginatedList = new PaginatedList<DepartmentDto>(
                departmentDtos,
                totalCount,
                request.PageNumber,
                request.PageSize);

            return Result<PaginatedList<DepartmentDto>>.Success(paginatedList);
        }
        catch (FilterParsingException ex)
        {
            _logger.LogWarning(ex, "Invalid filter format: {Filter}", request.Filter);
            return Result<PaginatedList<DepartmentDto>>.Failure($"Geçersiz filter formatı: {ex.Message}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving department list");
            return Result<PaginatedList<DepartmentDto>>.Failure("Bölüm listesi alınırken bir hata oluştu.");
        }
    }
}