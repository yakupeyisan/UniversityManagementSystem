using System.Linq.Expressions;
using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using UniversityMS.Application.Common.Extensions;
using UniversityMS.Application.Common.Models;
using UniversityMS.Application.Features.Departments.DTOs;
using UniversityMS.Domain.Entities.AcademicAggregate;
using UniversityMS.Domain.Interfaces;

namespace UniversityMS.Application.Features.Departments.Queries;

public class GetDepartmentListQueryHandler : IRequestHandler<GetDepartmentListQuery, Result<PaginatedList<DepartmentDto>>>
{
    private readonly IRepository<Department> _departmentRepository;
    private readonly IMapper _mapper;
    private readonly ILogger<GetDepartmentListQueryHandler> _logger;

    public GetDepartmentListQueryHandler(
        IRepository<Department> departmentRepository,
        IMapper mapper,
        ILogger<GetDepartmentListQueryHandler> logger)
    {
        _departmentRepository = departmentRepository;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<Result<PaginatedList<DepartmentDto>>> Handle(
        GetDepartmentListQuery request,
        CancellationToken cancellationToken)
    {
        try
        {
            Expression<Func<Department, bool>> predicate = d => !d.IsDeleted;

            if (request.FacultyId.HasValue)
            {
                var facultyId = request.FacultyId.Value;
                predicate = predicate.And(d => d.FacultyId == facultyId);
            }

            if (request.IsActive.HasValue)
            {
                var isActive = request.IsActive.Value;
                predicate = predicate.And(d => d.IsActive == isActive);
            }

            var (departments, totalCount) = await _departmentRepository.GetPagedAsync(
                request.PageNumber,
                request.PageSize,
                predicate,
                cancellationToken);

            var departmentDtos = _mapper.Map<List<DepartmentDto>>(departments);
            var paginatedList = new PaginatedList<DepartmentDto>(
                departmentDtos,
                totalCount,
                request.PageNumber,
                request.PageSize);

            return Result<PaginatedList<DepartmentDto>>.Success(paginatedList);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving department list");
            return Result<PaginatedList<DepartmentDto>>.Failure("Bölüm listesi alınırken bir hata oluştu.");
        }
    }
}