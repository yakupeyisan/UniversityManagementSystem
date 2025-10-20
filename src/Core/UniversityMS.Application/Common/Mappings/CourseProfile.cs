using AutoMapper;
using UniversityMS.Application.Features.CourseFeature.DTOs;
using UniversityMS.Domain.Entities.AcademicAggregate;

namespace UniversityMS.Application.Common.Mappings;

public class CourseProfile : Profile
{
    public CourseProfile()
    {
        // ============= COURSE MAPPING =============
        CreateMap<Course, CourseDto>()
            .ForMember(dest => dest.DepartmentName,
                opt => opt.MapFrom(src => src.Department.Name))
            .ForMember(dest => dest.StudentCount,
                opt => opt.MapFrom(src => 0)) // Repository'den alınması gerekebilir
            .ReverseMap()
            .ForMember(dest => dest.Department, opt => opt.Ignore());

        // ============= PREREQUISITE MAPPING =============
        CreateMap<Prerequisite, PrerequisiteDto>()
            .ForMember(dest => dest.PrerequisiteCourseName,
                opt => opt.MapFrom(src => src.PrerequisiteCourse.Name))
            .ForMember(dest => dest.PrerequisiteCourseCode,
                opt => opt.MapFrom(src => src.PrerequisiteCourse.Code))
            .ForMember(dest => dest.PrerequisiteCourseId,
                opt => opt.MapFrom(src => src.PrerequisiteCourseId))
            .ReverseMap()
            .ForMember(dest => dest.PrerequisiteCourse, opt => opt.Ignore());
    }
}
