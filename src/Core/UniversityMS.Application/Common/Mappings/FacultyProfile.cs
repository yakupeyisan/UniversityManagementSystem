using AutoMapper;
using UniversityMS.Application.Features.FacultyFeature.DTOs;
using UniversityMS.Domain.Entities.AcademicAggregate;

namespace UniversityMS.Application.Common.Mappings;

public class FacultyProfile : Profile
{
    public FacultyProfile()
    {
        // ============= FACULTY MAPPING =============
        CreateMap<Faculty, FacultyDto>()
            .ForMember(dest => dest.DepartmentCount,
                opt => opt.MapFrom(src => src.Departments.Count))
            .ForMember(dest => dest.CreatedAt,
                opt => opt.MapFrom(src => src.CreatedAt))
            .ReverseMap()
            .ForMember(dest => dest.Departments, opt => opt.Ignore());

        // ============= FACULTY DETAIL MAPPING =============
        CreateMap<Faculty, FacultyDetailDto>()
            .ForMember(dest => dest.Departments,
                opt => opt.MapFrom(src => src.Departments))
            .ForMember(dest => dest.CreatedAt,
                opt => opt.MapFrom(src => src.CreatedAt))
            .ReverseMap()
            .ForMember(dest => dest.Departments, opt => opt.Ignore());
    }
}