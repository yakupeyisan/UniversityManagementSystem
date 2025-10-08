using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using System.Linq.Expressions;
using UniversityMS.Application.Common.Models;
using UniversityMS.Application.Extensions;
using UniversityMS.Application.Features.Faculties.DTOs;
using UniversityMS.Domain.Entities.AcademicAggregate;
using UniversityMS.Domain.Interfaces;

namespace UniversityMS.Application.Features.Faculties.Queries;

public record GetFacultyListQuery(
    int PageNumber = 1,
    int PageSize = 10,
    bool? IsActive = null
) : IRequest<Result<PaginatedList<FacultyDto>>>;

public class GetFacultyListQueryHandler : IRequestHandler<GetFacultyListQuery, Result<PaginatedList<FacultyDto>>>
{
    private readonly IRepository<Faculty> _facultyRepository;
    private readonly IMapper _mapper;
    private readonly ILogger<GetFacultyListQueryHandler> _logger;

    public GetFacultyListQueryHandler(
        IRepository<Faculty> facultyRepository,
        IMapper mapper,
        ILogger<GetFacultyListQueryHandler> logger)
    {
        _facultyRepository = facultyRepository;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<Result<PaginatedList<FacultyDto>>> Handle(
        GetFacultyListQuery request,
        CancellationToken cancellationToken)
    {
        try
        {
            Expression<Func<Faculty, bool>> predicate = f => !f.IsDeleted;

            if (request.IsActive.HasValue)
            {
                var isActive = request.IsActive.Value;
                predicate = predicate.And(f => f.IsActive == isActive);
            }

            var (faculties, totalCount) = await _facultyRepository.GetPagedAsync(
                request.PageNumber,
                request.PageSize,
                predicate,
                cancellationToken);

            var facultyDtos = _mapper.Map<List<FacultyDto>>(faculties);
            var paginatedList = new PaginatedList<FacultyDto>(
                facultyDtos,
                totalCount,
                request.PageNumber,
                request.PageSize);

            return Result.Success(paginatedList);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving faculty list");
            return Result.Failure<PaginatedList<FacultyDto>>("Fakülte listesi alınırken bir hata oluştu.");
        }
    }
}


public record GetFacultyByIdQuery(Guid Id) : IRequest<Result<FacultyDto>>;

public class GetFacultyByIdQueryHandler : IRequestHandler<GetFacultyByIdQuery, Result<FacultyDto>>
{
    private readonly IRepository<Faculty> _facultyRepository;
    private readonly IMapper _mapper;
    private readonly ILogger<GetFacultyByIdQueryHandler> _logger;

    public GetFacultyByIdQueryHandler(
        IRepository<Faculty> facultyRepository,
        IMapper mapper,
        ILogger<GetFacultyByIdQueryHandler> logger)
    {
        _facultyRepository = facultyRepository;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<Result<FacultyDto>> Handle(GetFacultyByIdQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var faculty = await _facultyRepository.GetByIdAsync(request.Id, cancellationToken);

            if (faculty == null)
            {
                _logger.LogWarning("Faculty not found. FacultyId: {FacultyId}", request.Id);
                return Result.Failure<FacultyDto>("Fakülte bulunamadı.");
            }

            var facultyDto = _mapper.Map<FacultyDto>(faculty);
            return Result.Success(facultyDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving faculty. FacultyId: {FacultyId}", request.Id);
            return Result.Failure<FacultyDto>("Fakülte bilgileri alınırken bir hata oluştu.");
        }
    }
}