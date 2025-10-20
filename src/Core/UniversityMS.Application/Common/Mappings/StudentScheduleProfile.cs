using AutoMapper;
using UniversityMS.Application.Features.StudentFeature.DTOs;
using UniversityMS.Domain.Entities.ScheduleAggregate;

namespace UniversityMS.Application.Common.Mappings;

public class StudentScheduleProfile : Profile
{
    public StudentScheduleProfile()
    {
        // ============= TODAY COURSE DTO =============
        // NOT: TodayCourseDto'da StartTime ve EndTime TIMESPAN'dir, string değildir!
        CreateMap<CourseSession, TodayCourseDto>()
            .ForMember(dest => dest.CourseName,
                opt => opt.MapFrom(src => src.Course.Name))
            .ForMember(dest => dest.InstructorName,
                opt => opt.MapFrom(src => src.Instructor != null
                    ? $"{src.Instructor.FirstName} {src.Instructor.LastName}"
                    : "Atanmamış"))
            .ForMember(dest => dest.Classroom,
                opt => opt.MapFrom(src => src.Classroom.Name))
            .ForMember(dest => dest.StartTime,
                opt => opt.MapFrom(src => src.TimeSlot.StartTime))  // ✅ TimeSpan olarak
            .ForMember(dest => dest.EndTime,
                opt => opt.MapFrom(src => src.TimeSlot.EndTime))    // ✅ TimeSpan olarak
            .ReverseMap()
            .ForMember(dest => dest.Course, opt => opt.Ignore())
            .ForMember(dest => dest.Instructor, opt => opt.Ignore())
            .ForMember(dest => dest.Classroom, opt => opt.Ignore())
            .ForMember(dest => dest.TimeSlot, opt => opt.Ignore())
            .ForMember(dest => dest.Schedule, opt => opt.Ignore())
            .ForMember(dest => dest.ScheduleId, opt => opt.Ignore())
            .ForMember(dest => dest.CourseId, opt => opt.Ignore())
            .ForMember(dest => dest.InstructorId, opt => opt.Ignore())
            .ForMember(dest => dest.ClassroomId, opt => opt.Ignore())
            .ForMember(dest => dest.DayOfWeek, opt => opt.Ignore())
            .ForMember(dest => dest.SessionType, opt => opt.Ignore())
            .ForMember(dest => dest.Notes, opt => opt.Ignore())
            .ForMember(dest => dest.IsDeleted, opt => opt.Ignore())
            .ForMember(dest => dest.DeletedAt, opt => opt.Ignore())
            .ForMember(dest => dest.DeletedBy, opt => opt.Ignore());
    }
}