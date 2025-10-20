using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using UniversityMS.Application.Common.Models;
using UniversityMS.Application.Features.HRFeature.DTOs;
using UniversityMS.Domain.Entities.HRAggregate;
using UniversityMS.Domain.Filters;
using UniversityMS.Domain.Interfaces;
using UniversityMS.Domain.Specifications;

namespace UniversityMS.Application.Features.HRFeature.Queries;

public class GetAllEmployeesQueryHandler : IRequestHandler<GetAllEmployeesQuery, Result<PaginatedList<EmployeeListDto>>>
{
    private readonly IRepository<Employee> _employeeRepository;
    private readonly IFilterParser<Employee> _filterParser;
    private readonly IMapper _mapper;
    private readonly ILogger<GetAllEmployeesQueryHandler> _logger;

    public GetAllEmployeesQueryHandler(
        IRepository<Employee> employeeRepository,
        IFilterParser<Employee> filterParser,
        IMapper mapper,
        ILogger<GetAllEmployeesQueryHandler> logger)
    {
        _employeeRepository = employeeRepository;
        _filterParser = filterParser;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<Result<PaginatedList<EmployeeListDto>>> Handle(
        GetAllEmployeesQuery request,
        CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation(
                "Employee list requested. PageNumber: {PageNumber}, PageSize: {PageSize}, Filter: {Filter}",
                request.PageNumber,
                request.PageSize,
                request.Filter ?? "None");

            var specification = new EmployeeFilteredSpecification(
                filterString: request.Filter,
                pageNumber: request.PageNumber,
                pageSize: request.PageSize,
                filterParser: _filterParser);

            var employees = await _employeeRepository.ListAsync(specification, cancellationToken);

            var totalCount = await _employeeRepository.CountAsync(specification, cancellationToken);

            _logger.LogInformation(
                "Employees retrieved. TotalCount: {TotalCount}, Returned: {ReturnedCount}",
                totalCount,
                employees.Count);

            var dtos = _mapper.Map<List<EmployeeListDto>>(employees);

            var paginatedList = new PaginatedList<EmployeeListDto>(
                dtos,
                totalCount,
                request.PageNumber,
                request.PageSize);

            return Result<PaginatedList<EmployeeListDto>>.Success(paginatedList);
        }
        catch (FilterParsingException ex)
        {
            _logger.LogWarning(ex, "Invalid filter format: {Filter}", request.Filter);
            return Result<PaginatedList<EmployeeListDto>>.Failure(
                $"Geçersiz filter formatı: {ex.Message}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving employee list");
            return Result<PaginatedList<EmployeeListDto>>.Failure(
                "Çalışan listesi alınırken bir hata oluştu.");
        }
    }
}