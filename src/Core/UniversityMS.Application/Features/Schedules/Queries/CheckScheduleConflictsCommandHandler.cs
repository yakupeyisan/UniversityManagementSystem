using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using UniversityMS.Application.Common.Interfaces;
using UniversityMS.Application.Common.Models;
using UniversityMS.Application.Features.Schedules.DTOs;

namespace UniversityMS.Application.Features.Schedules.Queries;

public class CheckScheduleConflictsCommandHandler : IRequestHandler<CheckScheduleConflictsCommand, Result<ScheduleConflictDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly ILogger<CheckScheduleConflictsCommandHandler> _logger;

    public CheckScheduleConflictsCommandHandler(
        IApplicationDbContext context,
        ILogger<CheckScheduleConflictsCommandHandler> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<Result<ScheduleConflictDto>> Handle(CheckScheduleConflictsCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var schedule = await _context.Schedules
                .Include(s => s.CourseSessions)
                .ThenInclude(cs => cs.Course)
                .FirstOrDefaultAsync(s => s.Id == request.ScheduleId, cancellationToken);

            if (schedule == null)
                return Result<ScheduleConflictDto>.Failure("Program bulunamadı.");

            // Parse times
            if (!TimeSpan.TryParse(request.StartTime, out var startTime))
                return Result<ScheduleConflictDto>.Failure("Geçersiz başlangıç saati.");

            if (!TimeSpan.TryParse(request.EndTime, out var endTime))
                return Result<ScheduleConflictDto>.Failure("Geçersiz bitiş saati.");

            var timeSlot = Domain.ValueObjects.TimeSlot.Create(startTime, endTime);

            // Check classroom conflicts
            var classroomConflict = schedule.CourseSessions.FirstOrDefault(cs =>
                cs.ClassroomId == request.ClassroomId &&
                cs.DayOfWeek == (Domain.Enums.DayOfWeekEnum)request.DayOfWeek &&
                cs.TimeSlot.ConflictsWith(timeSlot) &&
                !cs.IsDeleted
            );

            if (classroomConflict != null)
            {
                return Result<ScheduleConflictDto>.Success(new ScheduleConflictDto
                {
                    HasConflict = true,
                    ConflictType = "Classroom",
                    ConflictMessage = "Bu zaman diliminde derslik dolu.",
                    ConflictingSession = new CourseSessionDto
                    {
                        CourseCode = classroomConflict.Course.Code,
                        CourseName = classroomConflict.Course.Name,
                        StartTime = classroomConflict.TimeSlot.StartTime.ToString(@"hh\:mm"),
                        EndTime = classroomConflict.TimeSlot.EndTime.ToString(@"hh\:mm")
                    }
                });
            }

            // Check instructor conflicts
            if (request.InstructorId.HasValue)
            {
                var instructorConflict = schedule.CourseSessions.FirstOrDefault(cs =>
                    cs.InstructorId == request.InstructorId &&
                    cs.DayOfWeek == (Domain.Enums.DayOfWeekEnum)request.DayOfWeek &&
                    cs.TimeSlot.ConflictsWith(timeSlot) &&
                    !cs.IsDeleted
                );

                if (instructorConflict != null)
                {
                    return Result<ScheduleConflictDto>.Success(new ScheduleConflictDto
                    {
                        HasConflict = true,
                        ConflictType = "Instructor",
                        ConflictMessage = "Öğretim üyesinin bu zaman diliminde başka dersi var.",
                        ConflictingSession = new CourseSessionDto
                        {
                            CourseCode = instructorConflict.Course.Code,
                            CourseName = instructorConflict.Course.Name,
                            StartTime = instructorConflict.TimeSlot.StartTime.ToString(@"hh\:mm"),
                            EndTime = instructorConflict.TimeSlot.EndTime.ToString(@"hh\:mm")
                        }
                    });
                }
            }

            return Result<ScheduleConflictDto>.Success(new ScheduleConflictDto
            {
                HasConflict = false,
                ConflictMessage = "Çakışma yok."
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking conflicts");
            return Result<ScheduleConflictDto>.Failure("Çakışma kontrolü yapılırken hata oluştu.");
        }
    }
}