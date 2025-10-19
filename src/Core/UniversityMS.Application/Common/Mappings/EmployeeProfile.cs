using AutoMapper;
using UniversityMS.Application.Features.StaffFeature.DTOs;
using UniversityMS.Domain.Entities.PersonAggregate;

namespace UniversityMS.Application.Common.Mappings;

public class EmployeeProfile : Profile
{
    public EmployeeProfile()
    {
        CreateMap<Staff, EmployeeDto>()
            .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email.Value))
            .ForMember(dest => dest.PhoneNumber, opt => opt.MapFrom(src => src.PhoneNumber.Value))
            .ReverseMap();
    }
}