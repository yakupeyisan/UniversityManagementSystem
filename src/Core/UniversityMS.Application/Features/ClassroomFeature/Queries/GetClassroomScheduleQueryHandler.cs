using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using UniversityMS.Application.Common.Interfaces;
using UniversityMS.Application.Common.Models;
using UniversityMS.Application.Features.ClassroomFeature.DTOs;
using UniversityMS.Application.Features.ScheduleFeature.DTOs;
using UniversityMS.Domain.Entities.FacilityAggregate;
using UniversityMS.Domain.Entities.ScheduleAggregate;
using UniversityMS.Domain.Enums;
using UniversityMS.Domain.Interfaces;

namespace UniversityMS.Application.Features.ClassroomFeature.Queries;
// ===============================================
// HATA 1-2: GradeProfile - Credits yerine ECTS ve NationalCredit
// ===============================================

// src/Core/UniversityMS.Application/Common/Mappings/GradeProfile.cs
using AutoMapper;
using global::UniversityMS.Application.Features.GradeFeature.DTOs;
using global::UniversityMS.Domain.Entities.EnrollmentAggregate;
using UniversityMS.Application.Features.GradeFeature.DTOs;
using UniversityMS.Domain.Entities.EnrollmentAggregate;

namespace UniversityMS.Application.Common.Mappings;

public class GradeProfile : Profile
{
    public GradeProfile()
    {
        CreateMap<Grade, GradeDetailDto>()
            .ForMember(dest => dest.CourseName, opt => opt.MapFrom(src => src.CourseRegistration.Course.Name))
            .ForMember(dest => dest.StudentId, opt => opt.MapFrom(src => src.CourseRegistration.Enrollment.StudentId))
            .ForMember(dest => dest.CourseId, opt => opt.MapFrom(src => src.CourseRegistration.CourseId))
            // ECTS olarak gönder, DTO'da Credits isminde
            .ForMember(dest => dest.Credits, opt => opt.MapFrom(src => src.CourseRegistration.ECTS))
            .ReverseMap();

        CreateMap<Grade, TranscriptCourseDto>()
            .ForMember(dest => dest.CourseCode, opt => opt.MapFrom(src => src.CourseRegistration.Course.Code))
            .ForMember(dest => dest.CourseName, opt => opt.MapFrom(src => src.CourseRegistration.Course.Name))
            // ECTS olarak gönder, DTO'da Credits isminde
            .ForMember(dest => dest.Credits, opt => opt.MapFrom(src => src.CourseRegistration.ECTS))
            .ForMember(dest => dest.CourseId, opt => opt.MapFrom(src => src.CourseRegistration.CourseId))
            .ForMember(dest => dest.AcademicYear, opt => opt.MapFrom(src => src.CourseRegistration.Enrollment.AcademicYear))
            .ForMember(dest => dest.Semester, opt => opt.MapFrom(src => src.CourseRegistration.Enrollment.Semester))
            .ReverseMap();

        CreateMap<GradeObjection, GradeObjectionDto>()
            .ReverseMap();
    }
}

// ===============================================
// HATA 3-6: CourseSession StartTime/EndTime Property'leri
// CourseSession'da string olarak kaydedilmiş, model'de de string
// ===============================================

// src/Core/UniversityMS.Application/Features/ClassroomFeature/Queries/GetClassroomScheduleQueryHandler.cs
using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using UniversityMS.Application.Common.Models;
using UniversityMS.Application.Features.ClassroomFeature.DTOs;
using UniversityMS.Application.Features.ScheduleFeature.DTOs;
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
            return Result<ClassroomScheduleDto>.Failure("Sınıf programı alınırken bir hata oluştu.", ex.Message);
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