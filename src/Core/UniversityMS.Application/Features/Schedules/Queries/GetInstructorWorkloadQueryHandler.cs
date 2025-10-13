using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using UniversityMS.Application.Common.Interfaces;
using UniversityMS.Application.Common.Models;
using UniversityMS.Application.Features.Schedules.DTOs;

namespace UniversityMS.Application.Features.Schedules.Queries;

public class GetInstructorWorkloadQueryHandler : IRequestHandler<GetInstructorWorkloadQuery, Result<InstructorWorkloadDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly ILogger<GetInstructorWorkloadQueryHandler> _logger;

    public GetInstructorWorkloadQueryHandler(
        IApplicationDbContext context,
        ILogger<GetInstructorWorkloadQueryHandler> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<Result<InstructorWorkloadDto>> Handle(GetInstructorWorkloadQuery request, CancellationToken cancellationToken)
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
                return Result.Failure<InstructorWorkloadDto>("Program bulunamadı.");

            var sessions = schedule.CourseSessions
                .Where(cs => cs.InstructorId == request.InstructorId && !cs.IsDeleted)
                .ToList();

            var totalMinutes = sessions.Sum(s => s.TimeSlot.DurationMinutes);
            var totalHours = totalMinutes / 60;

            var workload = new InstructorWorkloadDto
            {
                InstructorId = request.InstructorId,
                InstructorName = "Öğretim Üyesi", // TODO: Get from Staff entity
                TotalWeeklyHours = totalHours,
                TotalSessions = sessions.Count,
                Sessions = sessions.Select(s => new CourseSessionDto
                {
                    Id = s.Id,
                    ScheduleId = s.ScheduleId,
                    CourseId = s.CourseId,
                    CourseCode = s.Course.Code,
                    CourseName = s.Course.Name,
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
                }).ToList()
            };

            return Result.Success(workload);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving instructor workload");
            return Result.Failure<InstructorWorkloadDto>("Ders yükü getirilirken hata oluştu.");
        }
    }
}