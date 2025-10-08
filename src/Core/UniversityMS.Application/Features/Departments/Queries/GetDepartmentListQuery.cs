using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using System.Linq.Expressions;
using UniversityMS.Application.Common.Extensions;
using UniversityMS.Application.Common.Models;
using UniversityMS.Application.Features.Departments.DTOs;
using UniversityMS.Domain.Entities.AcademicAggregate;
using UniversityMS.Domain.Interfaces;

namespace UniversityMS.Application.Features.Departments.Queries;


public record GetDepartmentListQuery(
    int PageNumber = 1,
    int PageSize = 10,
    Guid? FacultyId = null,
    bool? IsActive = null
) : IRequest<Result<PaginatedList<DepartmentDto>>>;

public record GetDepartmentByIdQuery(Guid Id) : IRequest<Result<DepartmentDto>>;

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

            return Result.Success(paginatedList);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving department list");
            return Result.Failure<PaginatedList<DepartmentDto>>("Bölüm listesi alınırken bir hata oluştu.");
        }
    }
}

public class GetDepartmentByIdQueryHandler : IRequestHandler<GetDepartmentByIdQuery, Result<DepartmentDto>>
{
    private readonly IRepository<Department> _departmentRepository;
    private readonly IMapper _mapper;
    private readonly ILogger<GetDepartmentByIdQueryHandler> _logger;

    public GetDepartmentByIdQueryHandler(
        IRepository<Department> departmentRepository,
        IMapper mapper,
        ILogger<GetDepartmentByIdQueryHandler> logger)
    {
        _departmentRepository = departmentRepository;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<Result<DepartmentDto>> Handle(GetDepartmentByIdQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var department = await _departmentRepository.GetByIdAsync(request.Id, cancellationToken);

            if (department == null)
            {
                _logger.LogWarning("Department not found. DepartmentId: {DepartmentId}", request.Id);
                return Result.Failure<DepartmentDto>("Bölüm bulunamadı.");
            }

            var departmentDto = _mapper.Map<DepartmentDto>(department);
            return Result.Success(departmentDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving department. DepartmentId: {DepartmentId}", request.Id);
            return Result.Failure<DepartmentDto>("Bölüm bilgileri alınırken bir hata oluştu.");
        }
    }
}