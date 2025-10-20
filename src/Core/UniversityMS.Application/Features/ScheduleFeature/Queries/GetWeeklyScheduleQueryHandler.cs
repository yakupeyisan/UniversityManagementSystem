using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using UniversityMS.Application.Common.Models;
using UniversityMS.Application.Features.ScheduleFeature.DTOs;
using UniversityMS.Domain.Entities.ScheduleAggregate;
using UniversityMS.Domain.Interfaces;

namespace UniversityMS.Application.Features.ScheduleFeature.Queries;

public class GetWeeklyScheduleQueryHandler :
    IRequestHandler<GetWeeklyScheduleQuery, Result<WeeklyScheduleDto>>
{
    private readonly IRepository<Schedule> _scheduleRepository;
    private readonly IRepository<CourseSession> _courseSessionRepository;
    private readonly IMapper _mapper;
    private readonly ILogger<GetWeeklyScheduleQueryHandler> _logger;

    public GetWeeklyScheduleQueryHandler(
        IRepository<Schedule> scheduleRepository,
        IRepository<CourseSession> courseSessionRepository,
        IMapper mapper,
        ILogger<GetWeeklyScheduleQueryHandler> logger)
    {
        _scheduleRepository = scheduleRepository;
        _courseSessionRepository = courseSessionRepository;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<Result<WeeklyScheduleDto>> Handle(
        GetWeeklyScheduleQuery request,
        CancellationToken cancellationToken)
    {
        try
        {
            var schedule = await _scheduleRepository.GetByIdAsync(request.ScheduleId, cancellationToken);
            if (schedule == null)
                return Result<WeeklyScheduleDto>.Failure("Program bulunamadı.");

            var sessions = await _courseSessionRepository.FindAsync(
                cs => cs.ScheduleId == request.ScheduleId,
                cancellationToken);

            if (request.InstructorId.HasValue)
                sessions = sessions.Where(s => s.InstructorId == request.InstructorId).ToList();

            // CourseSessionExtendedDto kullan
            var dto = new WeeklyScheduleDto
            {
                ScheduleId = request.ScheduleId,
                AcademicYear = schedule.AcademicYear,
                Semester = schedule.Semester,
                Sessions = _mapper.Map<List<CourseSessionExtendedDto>>(
                    sessions.OrderBy(s => s.DayOfWeek).ThenBy(s => s.TimeSlot.StartTime))
            };

            return Result<WeeklyScheduleDto>.Success(dto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving weekly schedule");
            return Result<WeeklyScheduleDto>.Failure("Program bilgileri alınırken bir hata oluştu.");
        }
    }
}
