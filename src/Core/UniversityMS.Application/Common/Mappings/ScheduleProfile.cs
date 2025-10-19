using AutoMapper;
using UniversityMS.Application.Features.ScheduleFeature.DTOs;
using UniversityMS.Domain.Entities.ScheduleAggregate;

namespace UniversityMS.Application.Common.Mappings;

public class ScheduleProfile : Profile
{
    public ScheduleProfile()
    {
        CreateMap<CourseSession, CourseSessionExtendedDto>()
            .ForMember(dest => dest.CourseName, opt => opt.MapFrom(src => src.Course.Name))
            .ForMember(dest => dest.InstructorName, opt => opt.MapFrom(src =>
                src.Instructor != null
                    ? $"{src.Instructor.FirstName} {src.Instructor.LastName}"
                    : "N/A"))
            .ForMember(dest => dest.ClassroomName, opt => opt.MapFrom(src => src.Classroom.Name))
            .ReverseMap();
    }
}
