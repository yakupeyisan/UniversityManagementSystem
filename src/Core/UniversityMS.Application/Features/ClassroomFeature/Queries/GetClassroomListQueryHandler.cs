using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using UniversityMS.Application.Common.Interfaces;
using UniversityMS.Application.Common.Models;
using UniversityMS.Application.Features.ScheduleFeature.DTOs;
using UniversityMS.Domain.Entities.FacilityAggregate;
using UniversityMS.Domain.Filters;
using UniversityMS.Domain.Interfaces;
using UniversityMS.Domain.Specifications;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace UniversityMS.Application.Features.ClassroomFeature.Queries;

public class GetClassroomListQueryHandler : IRequestHandler<GetClassroomListQuery, Result<PaginatedList<ClassroomDto>>>
{
    private readonly IRepository<Classroom> _repository;
    private readonly IMapper _mapper;
    private readonly ILogger<GetClassroomListQueryHandler> _logger;
    private readonly IFilterParser<Classroom> _filterParser;

    public GetClassroomListQueryHandler(
        IRepository<Classroom> repository,
        IMapper mapper,
        IFilterParser<Classroom> filterParser,
        ILogger<GetClassroomListQueryHandler> logger)
    {
        _repository = repository;
        _mapper = mapper;
        _filterParser = filterParser;
        _logger = logger;
    }

    public async Task<Result<PaginatedList<ClassroomDto>>> Handle(GetClassroomListQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var specification = new ClassroomFilteredSpecification(
                filterString: request.Filter,
                pageNumber: request.PageNumber,
                pageSize: request.PageSize,
                filterParser: _filterParser);

            var classrooms = await _repository.ListAsync(specification, cancellationToken);

            var totalCount = await _repository.CountAsync(specification, cancellationToken);

            var dtos = _mapper.Map<List<ClassroomDto>>(classrooms);

            var paginatedList = new PaginatedList<ClassroomDto>(
                dtos,
                totalCount,
                request.PageNumber,
                request.PageSize
            );

            return Result<PaginatedList<ClassroomDto>>.Success(paginatedList);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving classroom list");
            return Result<PaginatedList<ClassroomDto>>.Failure("Derslik listesi getirilirken hata oluştu.");
        }
    }
}