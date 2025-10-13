using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using UniversityMS.Application.Common.Interfaces;
using UniversityMS.Application.Common.Models;
using UniversityMS.Application.Features.Schedules.DTOs;

namespace UniversityMS.Application.Features.Classrooms.Queries;

public class GetClassroomByIdQueryHandler : IRequestHandler<GetClassroomByIdQuery, Result<ClassroomDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;
    private readonly ILogger<GetClassroomByIdQueryHandler> _logger;

    public GetClassroomByIdQueryHandler(
        IApplicationDbContext context,
        IMapper mapper,
        ILogger<GetClassroomByIdQueryHandler> logger)
    {
        _context = context;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<Result<ClassroomDto>> Handle(GetClassroomByIdQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var classroom = await _context.Classrooms
                .FirstOrDefaultAsync(c => c.Id == request.Id && !c.IsDeleted, cancellationToken);

            if (classroom == null)
                return Result.Failure<ClassroomDto>("Derslik bulunamadı.");

            var dto = _mapper.Map<ClassroomDto>(classroom);

            return Result.Success(dto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving classroom");
            return Result.Failure<ClassroomDto>("Derslik getirilirken hata oluştu.");
        }
    }
}