using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using UniversityMS.Application.Common.Models;
using UniversityMS.Application.Features.HRFeature.DTOs;
using UniversityMS.Domain.Entities.HRAggregate;
using UniversityMS.Domain.Enums;
using UniversityMS.Domain.Interfaces;
using UniversityMS.Domain.Specifications;

namespace UniversityMS.Application.Features.HRFeature.Queries;

public class GetActiveEmployeesQueryHandler : IRequestHandler<GetActiveEmployeesQuery, Result<PaginatedList<EmployeeListDto>>>
{
    private readonly IRepository<Employee> _employeeRepository;
    private readonly IMapper _mapper;
    private readonly ILogger<GetActiveEmployeesQueryHandler> _logger;

    public GetActiveEmployeesQueryHandler(
        IRepository<Employee> employeeRepository,
        IMapper mapper,
        ILogger<GetActiveEmployeesQueryHandler> logger)
    {
        _employeeRepository = employeeRepository;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<Result<PaginatedList<EmployeeListDto>>> Handle(
        GetActiveEmployeesQuery request,
        CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation(
                "Active employees requested. PageNumber: {PageNumber}, PageSize: {PageSize}",
                request.PageNumber,
                request.PageSize);

            var specification = new ActiveEmployeesSpecification(
                EmploymentStatus.Active,
                request.PageNumber,
                request.PageSize);

            var employees = await _employeeRepository.ListAsync(specification, cancellationToken);
            var totalCount = await _employeeRepository.CountAsync(specification, cancellationToken);

            _logger.LogInformation(
                "Active employees retrieved. Total: {Total}, Returned: {Returned}",
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
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving active employees");
            return Result<PaginatedList<EmployeeListDto>>.Failure("Aktif çalışanlar alınırken hata oluştu.");
        }
    }
}