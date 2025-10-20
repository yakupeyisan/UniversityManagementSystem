using MediatR;
using Microsoft.Extensions.Logging;
using UniversityMS.Application.Common.Models;
using UniversityMS.Application.Features.ScheduleFeature.DTOs;
using UniversityMS.Domain.Entities.ScheduleAggregate;
using UniversityMS.Domain.Interfaces;

namespace UniversityMS.Application.Features.ScheduleFeature.Queries;

public class GetInstructorWorkloadQueryHandler :
    IRequestHandler<GetInstructorWorkloadQuery, Result<InstructorWorkloadDto>>
{
    private readonly IRepository<CourseSession> _courseSessionRepository;
    private readonly IRepository<Schedule> _scheduleRepository;
    private readonly ILogger<GetInstructorWorkloadQueryHandler> _logger;

    public GetInstructorWorkloadQueryHandler(
        IRepository<CourseSession> courseSessionRepository,
        IRepository<Schedule> scheduleRepository,
        ILogger<GetInstructorWorkloadQueryHandler> logger)
    {
        _courseSessionRepository = courseSessionRepository;
        _scheduleRepository = scheduleRepository;
        _logger = logger;
    }

    public async Task<Result<InstructorWorkloadDto>> Handle(
        GetInstructorWorkloadQuery request,
        CancellationToken cancellationToken)
    {
        try
        {
            var schedules = await _scheduleRepository.FindAsync(
                s => s.AcademicYear == request.AcademicYear && s.Semester == request.Semester,
                cancellationToken);

            if (!schedules.Any())
                return Result<InstructorWorkloadDto>.Failure("Program bulunamadı.");

            var schedule = schedules.First();
            var sessions = await _courseSessionRepository.FindAsync(
                cs => cs.ScheduleId == schedule.Id && cs.InstructorId == request.InstructorId,
                cancellationToken);

            // IReadOnlyList → List dönüştürme
            var sessionsList = sessions.ToList();

            var totalHours = CalculateTotalHours(sessionsList);
            var coursesPerWeek = sessionsList
                .GroupBy(s => new { s.DayOfWeek, s.TimeSlot.StartTime })
                .Count();

            var workload = new InstructorWorkloadDto
            {
                InstructorId = request.InstructorId,
                AcademicYear = request.AcademicYear,
                Semester = request.Semester,
                TotalCourses = sessionsList.Select(s => s.CourseId).Distinct().Count(),
                TotalHoursPerWeek = totalHours,
                SessionsPerWeek = coursesPerWeek,
                IsOverloaded = totalHours > 20,
                
            };

            return Result<InstructorWorkloadDto>.Success(workload);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error calculating instructor workload");
            return Result<InstructorWorkloadDto>.Failure("Ders yükü hesaplanırken bir hata oluştu.");
        }
    }

    private int CalculateTotalHours(List<CourseSession> sessions)
    {
        var weeklyHours = sessions
            .Sum(s => (s.TimeSlot.EndTime - s.TimeSlot.StartTime).TotalHours);

        return (int)weeklyHours;
    }
}