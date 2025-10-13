using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using UniversityMS.Application.Common.Interfaces;
using UniversityMS.Application.Common.Models;
using UniversityMS.Application.Features.Schedules.DTOs;

namespace UniversityMS.Application.Features.Schedules.Queries;

public class GetScheduleByIdQueryHandler : IRequestHandler<GetScheduleByIdQuery, Result<ScheduleDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;
    private readonly ILogger<GetScheduleByIdQueryHandler> _logger;

    public GetScheduleByIdQueryHandler(
        IApplicationDbContext context,
        IMapper mapper,
        ILogger<GetScheduleByIdQueryHandler> logger)
    {
        _context = context;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<Result<ScheduleDto>> Handle(GetScheduleByIdQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var schedule = await _context.Schedules
                .Include(s => s.CourseSessions)
                .FirstOrDefaultAsync(s => s.Id == request.Id && !s.IsDeleted, cancellationToken);

            if (schedule == null)
                return Result.Failure<ScheduleDto>("Program bulunamadı.");

            var dto = new ScheduleDto
            {
                Id = schedule.Id,
                AcademicYear = schedule.AcademicYear,
                Semester = schedule.Semester,
                DepartmentId = schedule.DepartmentId,
                Name = schedule.Name,
                Description = schedule.Description,
                Status = schedule.Status,
                PublishedDate = schedule.PublishedDate,
                StartDate = schedule.StartDate,
                EndDate = schedule.EndDate,
                TotalSessions = schedule.CourseSessions.Count(cs => !cs.IsDeleted)
            };

            return Result.Success(dto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving schedule");
            return Result.Failure<ScheduleDto>("Program getirilirken hata oluştu.");
        }
    }
}