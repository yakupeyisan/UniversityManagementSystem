using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using UniversityMS.Application.Common.Models;
using UniversityMS.Application.Features.ClassroomFeature.DTOs;
using UniversityMS.Application.Features.ScheduleFeature.DTOs;
using UniversityMS.Domain.Entities.FacilityAggregate;
using UniversityMS.Domain.Entities.ScheduleAggregate;
using UniversityMS.Domain.Enums;
using UniversityMS.Domain.Interfaces;

namespace UniversityMS.Application.Features.ClassroomFeature.Queries;

public class GetClassroomScheduleQueryHandler : IRequestHandler<GetClassroomScheduleQuery, Result<ClassroomScheduleDto>>
{
    private readonly IRepository<CourseSession> _courseSessionRepository;
    private readonly IRepository<Classroom> _classroomRepository;
    private readonly IMapper _mapper;
    private readonly ILogger<GetClassroomScheduleQueryHandler> _logger;

    public GetClassroomScheduleQueryHandler(
        IRepository<CourseSession> courseSessionRepository,
        IRepository<Classroom> classroomRepository,
        IMapper mapper,
        ILogger<GetClassroomScheduleQueryHandler> logger)
    {
        _courseSessionRepository = courseSessionRepository;
        _classroomRepository = classroomRepository;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<Result<ClassroomScheduleDto>> Handle(
        GetClassroomScheduleQuery request,
        CancellationToken cancellationToken)
    {
        try
        {
            var classroom = await _classroomRepository.GetByIdAsync(request.ClassroomId, cancellationToken);
            if (classroom == null)
                return Result<ClassroomScheduleDto>.Failure("Sınıf bulunamadı.");

            var sessions = await _courseSessionRepository.FindAsync(
                cs => cs.ClassroomId == request.ClassroomId,
                cancellationToken);

            var sessionsByDay = new List<CourseSessionExtendedDto>();

            foreach (var session in sessions)
            {
                sessionsByDay.Add(new CourseSessionExtendedDto
                {
                    Id = session.Id,
                    CourseId = session.CourseId,
                    CourseCode = session.Course?.Code ?? "N/A",
                    CourseName = session.Course?.Name ?? "N/A",
                    InstructorId = session.InstructorId ?? Guid.Empty,
                    InstructorName = session.Instructor != null
                        ? $"{session.Instructor.FirstName} {session.Instructor.LastName}"
                        : "N/A",
                    ClassroomId = session.ClassroomId,
                    ClassroomCode = classroom.Code,
                    ClassroomName = classroom.Name,
                    DayOfWeek = session.DayOfWeek,
                    DayName = session.DayOfWeek.ToString(),
                    // StartTime ve EndTime string olarak kaydedilmiş
                    StartTime = session.StartTime,
                    EndTime = session.EndTime,
                    // Hesapla: "09:00" ve "10:50" format'ında
                    DurationMinutes = CalculateDuration(session.StartTime, session.EndTime),
                    SessionType = session.SessionType.ToString(),
                    Notes = session.Notes,
                    ScheduleId = session.ScheduleId
                });
            }

            var orderedSessions = sessionsByDay
                .OrderBy(s => GetDayOrder(s.DayOfWeek))
                .ThenBy(s => s.StartTime)
                .ToList();

            var dto = new ClassroomScheduleDto
            {
                ClassroomId = request.ClassroomId,
                ClassroomCode = classroom.Code,
                ClassroomName = classroom.Name,
                Capacity = classroom.Capacity,
                // ClassroomType enum'ı string'e çevir
                Type = classroom.Type.ToString(),
                Sessions = orderedSessions
            };

            return Result<ClassroomScheduleDto>.Success(dto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving classroom schedule");
            return Result<ClassroomScheduleDto>.Failure("Sınıf programı alınırken bir hata oluştu. Hata:" + ex.Message);
        }
    }

    private int CalculateDuration(string startTime, string endTime)
    {
        try
        {
            if (TimeSpan.TryParse(startTime, out var start) && TimeSpan.TryParse(endTime, out var end))
            {
                return (int)(end - start).TotalMinutes;
            }
            return 0;
        }
        catch
        {
            return 0;
        }
    }

    private int GetDayOrder(DayOfWeekEnum day)
    {
        return day switch
        {
            DayOfWeekEnum.Monday => 0,
            DayOfWeekEnum.Tuesday => 1,
            DayOfWeekEnum.Wednesday => 2,
            DayOfWeekEnum.Thursday => 3,
            DayOfWeekEnum.Friday => 4,
            DayOfWeekEnum.Saturday => 5,
            DayOfWeekEnum.Sunday => 6,
            _ => 7
        };
    }
}