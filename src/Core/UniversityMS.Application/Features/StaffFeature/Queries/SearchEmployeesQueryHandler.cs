using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using UniversityMS.Application.Common.Models;
using UniversityMS.Application.Features.StaffFeature.DTOs;
using UniversityMS.Domain.Entities.PersonAggregate;
using UniversityMS.Domain.Interfaces;

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
            var allStaff = await _staffRepository.GetAllAsync(cancellationToken);

            var filtered = allStaff.AsEnumerable();

            if (!string.IsNullOrWhiteSpace(request.SearchTerm))
            {
                filtered = filtered.Where(s =>
                    s.FirstName.Contains(request.SearchTerm, StringComparison.OrdinalIgnoreCase) ||
                    s.LastName.Contains(request.SearchTerm, StringComparison.OrdinalIgnoreCase) ||
                    s.Email.Value.Contains(request.SearchTerm, StringComparison.OrdinalIgnoreCase));
            }

            if (request.DepartmentId.HasValue)
                filtered = filtered.Where(s => s.DepartmentId == request.DepartmentId);

            if (!string.IsNullOrWhiteSpace(request.Position))
                filtered = filtered.Where(s => s.JobTitle == request.Position);

            var totalCount = filtered.Count();
            var paginatedStaff = filtered
                .OrderBy(s => s.FirstName)
                .Skip((request.PageNumber - 1) * request.PageSize)
                .Take(request.PageSize)
                .ToList();

            var dtos = _mapper.Map<List<EmployeeDto>>(paginatedStaff);

            var result = new PaginatedList<EmployeeDto>(dtos, totalCount, request.PageNumber, request.PageSize);

            return Result<PaginatedList<EmployeeDto>>.Success(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error searching employees");
            return Result<PaginatedList<EmployeeDto>>.Failure("Çalışan araması sırasında bir hata oluştu. Hata:" + ex.Message);
        }
    }
}
