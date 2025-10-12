using AutoMapper;
using UniversityMS.Application.Features.Authentication.DTOs;
using UniversityMS.Application.Features.Staff.DTOs;
using UniversityMS.Application.Features.Students.DTOs;
using UniversityMS.Domain.Entities.IdentityAggregate;
using UniversityMS.Domain.Entities.PersonAggregate;

namespace UniversityMS.Application.Common.Behaviours;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        // Student Mappings
        CreateMap<Student, StudentDto>()
            .ForMember(d => d.FullName, opt => opt.MapFrom(s => s.GetFullName()))
            .ForMember(d => d.Age, opt => opt.MapFrom(s => s.GetAge()))
            .ForMember(d => d.Email, opt => opt.MapFrom(s => s.Email.Value))
            .ForMember(d => d.PhoneNumber, opt => opt.MapFrom(s => s.PhoneNumber.Value));

        // Staff Mappings
        CreateMap<Staff, StaffDto>()
            .ForMember(d => d.FullName, opt => opt.MapFrom(s => s.GetFullName()))
            .ForMember(d => d.Age, opt => opt.MapFrom(s => s.GetAge()))
            .ForMember(d => d.Email, opt => opt.MapFrom(s => s.Email.Value))
            .ForMember(d => d.PhoneNumber, opt => opt.MapFrom(s => s.PhoneNumber.Value))
            .ForMember(d => d.YearsOfService, opt => opt.MapFrom(s => s.GetYearsOfService()));

        // User Mappings
        CreateMap<User, UserDto>()
            .ForMember(d => d.Email, opt => opt.MapFrom(s => s.Email.Value))
            .ForMember(d => d.Roles, opt => opt.MapFrom(s => s.UserRoles.Select(ur => ur.Role.Name)));

        // Role Mappings
        CreateMap<Role, RoleDto>();

        // Address Mappings
        CreateMap<Address, AddressDto>();

        // Emergency Contact Mappings
        CreateMap<EmergencyContact, EmergencyContactDto>()
            .ForMember(d => d.PhoneNumber, opt => opt.MapFrom(s => s.PhoneNumber.Value))
            .ForMember(d => d.AlternativePhone, opt => opt.MapFrom(s =>
                s.AlternativePhone != null ? s.AlternativePhone.Value : null));
    }
}