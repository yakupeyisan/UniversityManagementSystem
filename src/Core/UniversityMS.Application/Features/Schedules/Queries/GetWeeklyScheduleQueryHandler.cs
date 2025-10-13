using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using UniversityMS.Application.Common.Interfaces;
using UniversityMS.Application.Common.Models;
using UniversityMS.Application.Features.Schedules.DTOs;
using UniversityMS.Domain.Enums;

namespace UniversityMS.Application.Features.Schedules.Queries;

public class GetWeeklyScheduleQueryHandler : IRequestHandler<GetWeeklyScheduleQuery, Result<WeeklyScheduleDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;
    private readonly ILogger<GetWeeklyScheduleQueryHandler> _logger;

    public GetWeeklyScheduleQueryHandler(
        IApplicationDbContext context,
        IMapper mapper,
        ILogger<GetWeeklyScheduleQueryHandler> logger)
    {
        _context = context;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<Result<WeeklyScheduleDto>> Handle(GetWeeklyScheduleQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var schedule = await _context.Schedules
                .Include(s => s.CourseSessions)
                .ThenInclude(cs => cs.Course)
                .Include(s => s.CourseSessions)
                .ThenInclude(cs => cs.Classroom)
                .FirstOrDefaultAsync(s => s.Id == request.ScheduleId && !s.IsDeleted, cancellationToken);

            if (schedule == null)
                return Result.Failure<WeeklyScheduleDto>("Program bulunamadı.");

            var sessionsQuery = schedule.CourseSessions.Where(cs => !cs.IsDeleted).AsQueryable();

            // Filter by instructor if provided
            if (request.InstructorId.HasValue)
            {
                sessionsQuery = sessionsQuery.Where(cs => cs.InstructorId == request.InstructorId.Value);
            }

            // Filter by student if provided (need to check enrollments)
            if (request.StudentId.HasValue)
            {
                var studentCourseIds = await _context.Enrollments
                    .Where(e => e.StudentId == request.StudentId.Value &&
                                e.AcademicYear == schedule.AcademicYear &&
                                e.Semester == schedule.Semester)
                    .SelectMany(e => e.CourseRegistrations)
                    .Select(cr => cr.CourseId)
                    .ToListAsync(cancellationToken);

                sessionsQuery = sessionsQuery.Where(cs => studentCourseIds.Contains(cs.CourseId));
            }

            var sessions = sessionsQuery.ToList();

            var weeklySchedule = new WeeklyScheduleDto
            {
                ScheduleId = schedule.Id,
                AcademicYear = schedule.AcademicYear,
                Semester = schedule.Semester,
                Sessions = new Dictionary<DayOfWeekEnum, List<CourseSessionDto>>()
            };

            // Group by day of week
            foreach (DayOfWeekEnum day in Enum.GetValues(typeof(DayOfWeekEnum)))
            {
                var daySessions = sessions
                    .Where(s => s.DayOfWeek == day)
                    .OrderBy(s => s.TimeSlot.StartTime)
                    .Select(s => new CourseSessionDto
                    {
                        Id = s.Id,
                        ScheduleId = s.ScheduleId,
                        CourseId = s.CourseId,
                        CourseCode = s.Course.Code,
                        CourseName = s.Course.Name,
                        InstructorId = s.InstructorId,
                        ClassroomId = s.ClassroomId,
                        ClassroomCode = s.Classroom.Code,
                        ClassroomName = s.Classroom.Name,
                        DayOfWeek = s.DayOfWeek,
                        DayName = s.GetDayName(),
                        StartTime = s.TimeSlot.StartTime.ToString(@"hh\:mm"),
                        EndTime = s.TimeSlot.EndTime.ToString(@"hh\:mm"),
                        DurationMinutes = s.TimeSlot.DurationMinutes,
                        SessionType = s.SessionType,
                        Notes = s.Notes
                    })
                    .ToList();

                weeklySchedule.Sessions[day] = daySessions;
            }

            return Result.Success(weeklySchedule);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving weekly schedule");
            return Result.Failure<WeeklyScheduleDto>("Program getirilirken hata oluştu.");
        }
    }
}