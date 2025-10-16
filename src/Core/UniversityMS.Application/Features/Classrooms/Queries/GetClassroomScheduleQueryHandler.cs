using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using UniversityMS.Application.Common.Interfaces;
using UniversityMS.Application.Common.Models;
using UniversityMS.Application.Features.Schedules.DTOs;
using UniversityMS.Domain.Enums;

namespace UniversityMS.Application.Features.Classrooms.Queries;

public class GetClassroomScheduleQueryHandler : IRequestHandler<GetClassroomScheduleQuery, Result<WeeklyScheduleDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly ILogger<GetClassroomScheduleQueryHandler> _logger;

    public GetClassroomScheduleQueryHandler(
        IApplicationDbContext context,
        ILogger<GetClassroomScheduleQueryHandler> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<Result<WeeklyScheduleDto>> Handle(GetClassroomScheduleQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var schedule = await _context.Schedules
                .Include(s => s.CourseSessions)
                .ThenInclude(cs => cs.Course)
                .Include(s => s.CourseSessions)
                .ThenInclude(cs => cs.Classroom)
                .FirstOrDefaultAsync(s =>
                        s.AcademicYear == request.AcademicYear &&
                        s.Semester == request.Semester &&
                        !s.IsDeleted,
                    cancellationToken);

            if (schedule == null)
                return Result<WeeklyScheduleDto>.Failure("Program bulunamadı.");

            var sessions = schedule.CourseSessions
                .Where(cs => cs.ClassroomId == request.ClassroomId && !cs.IsDeleted)
                .ToList();

            var weeklySchedule = new WeeklyScheduleDto
            {
                ScheduleId = schedule.Id,
                AcademicYear = schedule.AcademicYear,
                Semester = schedule.Semester,
                Sessions = new Dictionary<DayOfWeekEnum, List<CourseSessionDto>>()
            };

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

            return Result<WeeklyScheduleDto>.Success(weeklySchedule);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving classroom schedule");
            return Result<WeeklyScheduleDto>.Failure("Derslik programı getirilirken hata oluştu.");
        }
    }
}