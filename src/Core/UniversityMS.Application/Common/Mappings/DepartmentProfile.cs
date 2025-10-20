using AutoMapper;
using UniversityMS.Application.Features.DepartmentFeature.DTOs;
using UniversityMS.Domain.Entities.AcademicAggregate;

namespace UniversityMS.Application.Common.Mappings;

public class DepartmentProfile : Profile
{
    public DepartmentProfile()
    {
        // ============= DEPARTMENT MAPPING =============
        CreateMap<Department, DepartmentDto>()
            .ForMember(dest => dest.FacultyName,
                opt => opt.MapFrom(src => src.Faculty.Name))
            .ForMember(dest => dest.HeadId,
                opt => opt.MapFrom(src => src.HeadOfDepartmentId))
            .ForMember(dest => dest.HeadName,
                opt => opt.MapFrom(src => src.HeadOfDepartment != null
                    ? $"{src.HeadOfDepartment.FirstName} {src.HeadOfDepartment.LastName}"
                    : "Atanmamış"))
            .ForMember(dest => dest.CourseCount,
                opt => opt.MapFrom(src => src.Courses.Count))
            .ForMember(dest => dest.CreatedAt,
                opt => opt.MapFrom(src => src.CreatedAt))
            .ReverseMap()
            .ForMember(dest => dest.Faculty, opt => opt.Ignore())
            .ForMember(dest => dest.Courses, opt => opt.Ignore())
            .ForMember(dest => dest.HeadOfDepartment, opt => opt.Ignore());
    }
}