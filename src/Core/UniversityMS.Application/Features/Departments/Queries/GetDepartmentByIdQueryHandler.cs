using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using UniversityMS.Application.Common.Models;
using UniversityMS.Application.Features.Departments.DTOs;
using UniversityMS.Domain.Entities.AcademicAggregate;
using UniversityMS.Domain.Interfaces;

namespace UniversityMS.Application.Features.Departments.Queries;

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