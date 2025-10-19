using AutoMapper;
using UniversityMS.Application.Features.CourseFeature.DTOs;
using UniversityMS.Domain.Entities.AcademicAggregate;

namespace UniversityMS.Application.Common.Mappings;

public class CourseProfile : Profile
{
    public CourseProfile()
    {
        CreateMap<Course, CourseDto>()
            .ForMember(dest => dest.DepartmentName, opt => opt.MapFrom(src => src.Department.Name))
            .ReverseMap();

        CreateMap<Prerequisite, PrerequisiteDto>()
            .ForMember(dest => dest.PrerequisiteCourseName, opt => opt.MapFrom(src => src.PrerequisiteCourse.Name))
            .ForMember(dest => dest.PrerequisiteCourseCode, opt => opt.MapFrom(src => src.PrerequisiteCourse.Code))
            .ForMember(dest => dest.PrerequisiteCourseId, opt => opt.MapFrom(src => src.PrerequisiteCourseId))
            .ReverseMap();
    }
}