using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using UniversityMS.Application.Common.Interfaces;
using UniversityMS.Application.Common.Models;
using UniversityMS.Application.Features.ScheduleFeature.DTOs;

namespace UniversityMS.Application.Features.ClassroomFeature.Queries;

public class GetClassroomListQueryHandler : IRequestHandler<GetClassroomListQuery, Result<PaginatedList<ClassroomDto>>>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;
    private readonly ILogger<GetClassroomListQueryHandler> _logger;

    public GetClassroomListQueryHandler(
        IApplicationDbContext context,
        IMapper mapper,
        ILogger<GetClassroomListQueryHandler> logger)
    {
        _context = context;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<Result<PaginatedList<ClassroomDto>>> Handle(GetClassroomListQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var query = _context.Classrooms.Where(c => !c.IsDeleted);

            if (!string.IsNullOrWhiteSpace(request.Building))
            {
                query = query.Where(c => c.Building == request.Building);
            }

            if (request.IsActive.HasValue)
            {
                query = query.Where(c => c.IsActive == request.IsActive.Value);
            }

            var totalCount = await query.CountAsync(cancellationToken);

            var classrooms = await query
                .OrderBy(c => c.Building)
                .ThenBy(c => c.Floor)
                .ThenBy(c => c.Code)
                .Skip((request.PageNumber - 1) * request.PageSize)
                .Take(request.PageSize)
                .ToListAsync(cancellationToken);

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