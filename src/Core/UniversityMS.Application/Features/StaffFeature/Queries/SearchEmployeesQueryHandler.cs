using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using UniversityMS.Application.Common.Models;
using UniversityMS.Application.Features.StaffFeature.DTOs;
using UniversityMS.Domain.Entities.PersonAggregate;
using UniversityMS.Domain.Interfaces;
using UniversityMS.Domain.Specifications;

namespace UniversityMS.Application.Features.StaffFeature.Queries;

public class SearchEmployeesQueryHandler : IRequestHandler<SearchEmployeesQuery, Result<PaginatedList<EmployeeDto>>>
{
    private readonly IRepository<Staff> _staffRepository;
    private readonly IMapper _mapper;
    private readonly ILogger<SearchEmployeesQueryHandler> _logger;

    public SearchEmployeesQueryHandler(
        IRepository<Staff> staffRepository,
        IMapper mapper,
        ILogger<SearchEmployeesQueryHandler> logger)
    {
        _staffRepository = staffRepository;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<Result<PaginatedList<EmployeeDto>>> Handle(
        SearchEmployeesQuery request,
        CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation(
                "Staff search requested. SearchTerm: {SearchTerm}, DepartmentId: {DepartmentId}, Position: {Position}",
                request.SearchTerm ?? "None",
                request.DepartmentId,
                request.Position ?? "None");

            var specification = new SearchStaffSpecification(
                request.SearchTerm,
                request.DepartmentId,
                request.Position,
                request.PageNumber,
                request.PageSize);

            var staff = await _staffRepository.ListAsync(specification, cancellationToken);
            var totalCount = await _staffRepository.CountAsync(specification, cancellationToken);

            _logger.LogInformation(
                "Staff search completed. Total: {Total}, Returned: {Returned}",
                totalCount,
                staff.Count);

            var dtos = _mapper.Map<List<EmployeeDto>>(staff);

            var result = new PaginatedList<EmployeeDto>(
                dtos,
                totalCount,
                request.PageNumber,
                request.PageSize);

            return Result<PaginatedList<EmployeeDto>>.Success(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error searching employees");
            return Result<PaginatedList<EmployeeDto>>.Failure("Çalışan araması sırasında bir hata oluştu.");
        }
    }
}