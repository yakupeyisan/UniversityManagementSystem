using System.Linq.Expressions;
using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using UniversityMS.Application.Common.Extensions;
using UniversityMS.Application.Common.Models;
using UniversityMS.Application.Features.Faculties.DTOs;
using UniversityMS.Domain.Entities.AcademicAggregate;
using UniversityMS.Domain.Interfaces;

namespace UniversityMS.Application.Features.Faculties.Queries;

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