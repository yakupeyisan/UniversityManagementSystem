using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using System.Linq.Expressions;
using UniversityMS.Application.Common.Extensions;
using UniversityMS.Application.Common.Models;
using UniversityMS.Application.Features.FacultyFeature.DTOs;
using UniversityMS.Domain.Entities.AcademicAggregate;
using UniversityMS.Domain.Filters;
using UniversityMS.Domain.Interfaces;
using UniversityMS.Domain.Specifications;

namespace UniversityMS.Application.Features.FacultyFeature.Queries;
public class GetFacultyListQueryHandler : IRequestHandler<GetFacultyListQuery, Result<PaginatedList<FacultyDto>>>
{
    private readonly IRepository<Faculty> _facultyRepository;
    private readonly IFilterParser<Faculty> _filterParser;
    private readonly IMapper _mapper;
    private readonly ILogger<GetFacultyListQueryHandler> _logger;

    public GetFacultyListQueryHandler(
        IRepository<Faculty> facultyRepository,
        IFilterParser<Faculty> filterParser,
        IMapper mapper,
        ILogger<GetFacultyListQueryHandler> logger)
    {
        _facultyRepository = facultyRepository;
        _filterParser = filterParser;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<Result<PaginatedList<FacultyDto>>> Handle(
        GetFacultyListQuery request,
        CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation(
                "Faculty list requested. PageNumber: {PageNumber}, PageSize: {PageSize}, Filter: {Filter}",
                request.PageNumber,
                request.PageSize,
                request.Filter ?? "None");

            // ✅ SPECIFICATION PATTERN
            var specification = new FacultyFilteredSpecification(
                filterString: request.Filter,
                pageNumber: request.PageNumber,
                pageSize: request.PageSize,
                filterParser: _filterParser);

            var faculties = await _facultyRepository.ListAsync(specification, cancellationToken);
            var totalCount = await _facultyRepository.CountAsync(specification, cancellationToken);

            _logger.LogInformation(
                "Faculties retrieved. TotalCount: {TotalCount}, Returned: {ReturnedCount}",
                totalCount,
                faculties.Count);

            var facultyDtos = _mapper.Map<List<FacultyDto>>(faculties);
            var paginatedList = new PaginatedList<FacultyDto>(
                facultyDtos,
                totalCount,
                request.PageNumber,
                request.PageSize);

            return Result<PaginatedList<FacultyDto>>.Success(paginatedList);
        }
        catch (FilterParsingException ex)
        {
            _logger.LogWarning(ex, "Invalid filter format: {Filter}", request.Filter);
            return Result<PaginatedList<FacultyDto>>.Failure($"Geçersiz filter formatı: {ex.Message}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving faculty list");
            return Result<PaginatedList<FacultyDto>>.Failure("Fakülte listesi alınırken bir hata oluştu.");
        }
    }
}