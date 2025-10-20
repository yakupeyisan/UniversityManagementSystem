using AutoMapper;
using UniversityMS.Application.Features.StaffFeature.DTOs;
using UniversityMS.Domain.Entities.PersonAggregate;

namespace UniversityMS.Application.Common.Mappings;

/// <summary>
/// Staff → EmployeeDto mapping'i (Alternatif DTO)
/// </summary>
public class EmployeeProfile : Profile
{
    public EmployeeProfile()
    {
        CreateMap<Staff, EmployeeDto>()
            .ForMember(dest => dest.FirstName,
                opt => opt.MapFrom(src => src.FirstName))
            .ForMember(dest => dest.LastName,
                opt => opt.MapFrom(src => src.LastName))
            .ForMember(dest => dest.Email,
                opt => opt.MapFrom(src => src.Email.Value))
            .ForMember(dest => dest.PhoneNumber,
                opt => opt.MapFrom(src => src.PhoneNumber.Value))
            .ForMember(dest => dest.Position,
                opt => opt.MapFrom(src => src.JobTitle))
            .ForMember(dest => dest.Department,
                opt => opt.MapFrom(src => src.DepartmentId.HasValue ? "Department" : "N/A"))
            .ForMember(dest => dest.HireDate,
                opt => opt.MapFrom(src => src.HireDate))
            .ForMember(dest => dest.IsActive,
                opt => opt.MapFrom(src => src.IsActive))
            .ReverseMap()
            .ForMember(dest => dest.Email, opt => opt.Ignore())
            .ForMember(dest => dest.PhoneNumber, opt => opt.Ignore());
    }
}