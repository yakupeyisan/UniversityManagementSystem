using AutoMapper;
using UniversityMS.Application.Features.DepartmentFeature.DTOs;
using UniversityMS.Domain.Entities.AcademicAggregate;

namespace UniversityMS.Application.Common.Mappings;

public class DepartmentProfile : Profile
{
    public DepartmentProfile()
    {
        CreateMap<Department, DepartmentDto>()
            .ForMember(dest => dest.FacultyName, opt => opt.MapFrom(src => src.Faculty.Name))
            .ReverseMap();
    }
}