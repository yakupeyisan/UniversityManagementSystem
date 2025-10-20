using AutoMapper;
using UniversityMS.Application.Features.ScheduleFeature.DTOs;
using UniversityMS.Domain.Entities.ScheduleAggregate;

namespace UniversityMS.Application.Common.Mappings;
public class ScheduleProfile : Profile
{
    public ScheduleProfile()
    {
        // ============= COURSE SESSION EXTENDED DTO =============
        // ENTITY: CourseSession - TimeSlot Value Object'i içerir
        // DTO: CourseSessionExtendedDto - string olarak StartTime/EndTime tutar

        CreateMap<CourseSession, CourseSessionExtendedDto>()
            // TEMEL ALANLAR
            .ForMember(dest => dest.Id,
                opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.ScheduleId,
                opt => opt.MapFrom(src => src.ScheduleId))
            .ForMember(dest => dest.CourseId,
                opt => opt.MapFrom(src => src.CourseId))
            .ForMember(dest => dest.ClassroomId,
                opt => opt.MapFrom(src => src.ClassroomId))
            .ForMember(dest => dest.DayOfWeek,
                opt => opt.MapFrom(src => src.DayOfWeek))
            .ForMember(dest => dest.Notes,
                opt => opt.MapFrom(src => src.Notes))

            // ============= NAVIGATION PROPERTY MAPPINGS =============
            // Course
            .ForMember(dest => dest.CourseCode,
                opt => opt.MapFrom(src => src.Course != null ? src.Course.Code : "N/A"))
            .ForMember(dest => dest.CourseName,
                opt => opt.MapFrom(src => src.Course != null ? src.Course.Name : "N/A"))

            // Instructor (Nullable)
            .ForMember(dest => dest.InstructorId,
                opt => opt.MapFrom(src => src.InstructorId ?? Guid.Empty))
            .ForMember(dest => dest.InstructorName,
                opt => opt.MapFrom(src => src.Instructor != null
                    ? $"{src.Instructor.FirstName} {src.Instructor.LastName}"
                    : "Atanmamış"))

            // Classroom - ✅ DÜZELTME: RoomNumber yerine Code kullan
            .ForMember(dest => dest.ClassroomCode,
                opt => opt.MapFrom(src => src.Classroom != null ? src.Classroom.Code : "N/A"))
            .ForMember(dest => dest.ClassroomName,
                opt => opt.MapFrom(src => src.Classroom != null ? src.Classroom.Name : "N/A"))

            // ============= VALUE OBJECT MAPPING (ÖNEMLİ!) =============
            // ✅ TimeSlot Value Object'ten erişim
            .ForMember(dest => dest.StartTime,
                opt => opt.MapFrom(src => src.TimeSlot.StartTime.ToString(@"hh\:mm")))
            .ForMember(dest => dest.EndTime,
                opt => opt.MapFrom(src => src.TimeSlot.EndTime.ToString(@"hh\:mm")))
            .ForMember(dest => dest.DurationMinutes,
                opt => opt.MapFrom(src => src.TimeSlot.DurationMinutes))

            // ============= ENUM MAPPING =============
            .ForMember(dest => dest.SessionType,
                opt => opt.MapFrom(src => src.SessionType.ToString()))

            // ============= HESAPLANAN ALANLAR =============
            .ForMember(dest => dest.DayName,
                opt => opt.MapFrom(src => src.GetDayName()))

            // ============= REVERSE MAP =============
            // DTO'dan Entity'e dönüşüm (Entity'nin read-only property'leri ignore edilecek)
            .ReverseMap()
            .ForMember(dest => dest.Course, opt => opt.Ignore())
            .ForMember(dest => dest.Instructor, opt => opt.Ignore())
            .ForMember(dest => dest.Classroom, opt => opt.Ignore())
            .ForMember(dest => dest.Schedule, opt => opt.Ignore())
            .ForMember(dest => dest.TimeSlot, opt => opt.Ignore())  // Value Object'i ignore et
            .ForMember(dest => dest.SessionType, opt => opt.Ignore())
            .ForMember(dest => dest.IsDeleted, opt => opt.Ignore())
            .ForMember(dest => dest.DeletedAt, opt => opt.Ignore())
            .ForMember(dest => dest.DeletedBy, opt => opt.Ignore());

        // Schedule -> ScheduleDto
        CreateMap<Schedule, ScheduleDto>()
            .ForMember(dest => dest.SessionCount,
                opt => opt.MapFrom(src => src.CourseSessions.Count))
            .ReverseMap();

        // CourseSession -> CourseSessionDto
        CreateMap<CourseSession, CourseSessionDto>()
            .ForMember(dest => dest.StartTime,
                opt => opt.MapFrom(src => src.TimeSlot.StartTime))
            .ForMember(dest => dest.EndTime,
                opt => opt.MapFrom(src => src.TimeSlot.EndTime))
            .ForMember(dest => dest.CourseName,
                opt => opt.MapFrom(src => src.Course!.Name))
            .ForMember(dest => dest.CourseCode,
                opt => opt.MapFrom(src => src.Course!.Code))
            .ReverseMap();
    }
}
