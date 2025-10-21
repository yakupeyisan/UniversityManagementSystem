using AutoMapper;
using UniversityMS.Application.Features.AuthenticationFeature.DTOs;
using UniversityMS.Application.Features.StaffFeature.DTOs;
using UniversityMS.Application.Features.StudentFeature.DTOs;
using UniversityMS.Domain.Entities.IdentityAggregate;
using UniversityMS.Domain.Entities.PersonAggregate;

namespace UniversityMS.Application.Common.Mappings;

/// <summary>
/// Person entity'lerinden DTO'lara mapping'ler
/// Student, Staff, User entity'lerini handle eder
/// </summary>
public class PersonProfile : Profile
{
    public PersonProfile()
    {
        // ============= STUDENT MAPPING =============
        CreateMap<Student, StudentDto>()
            .ForMember(dest => dest.FullName,
                opt => opt.MapFrom(src => $"{src.FirstName} {src.LastName}"))
            .ForMember(dest => dest.Email,
                opt => opt.MapFrom(src => src.Email.Value))
            .ForMember(dest => dest.PhoneNumber,
                opt => opt.MapFrom(src => src.PhoneNumber.Value))
            .ForMember(dest => dest.Age,
                opt => opt.MapFrom(src => DateTime.UtcNow.Year - src.BirthDate.Year))
            .ReverseMap()
            .ForMember(dest => dest.Email, opt => opt.Ignore())
            .ForMember(dest => dest.PhoneNumber, opt => opt.Ignore());
        ;

        // ============= STAFF MAPPING =============
        CreateMap<Staff, StaffDto>()
            .ForMember(dest => dest.FullName,
                opt => opt.MapFrom(src => $"{src.FirstName} {src.LastName}"))
            .ForMember(dest => dest.Email,
                opt => opt.MapFrom(src => src.Email.Value))
            .ForMember(dest => dest.PhoneNumber,
                opt => opt.MapFrom(src => src.PhoneNumber.Value))
            .ForMember(dest => dest.Age,
                opt => opt.MapFrom(src => DateTime.UtcNow.Year - src.BirthDate.Year))
            .ForMember(dest => dest.YearsOfService,
                opt => opt.MapFrom(src => DateTime.UtcNow.Year - src.HireDate.Year))
            .ReverseMap()
            .ForMember(dest => dest.Email, opt => opt.Ignore())
            .ForMember(dest => dest.PhoneNumber, opt => opt.Ignore());

        // ============= USER MAPPING =============
        CreateMap<User, UserDto>()
            .ForMember(dest => dest.Username,
                opt => opt.MapFrom(src => src.Username))
            .ForMember(dest => dest.Email,
                opt => opt.MapFrom(src => src.Email.Value))
            .ForMember(dest => dest.FirstName,
                opt => opt.MapFrom(src => src.FirstName))
            .ForMember(dest => dest.LastName,
                opt => opt.MapFrom(src => src.LastName))
            .ForMember(dest => dest.IsActive,
                opt => opt.MapFrom(src => src.IsActive))
            .ForMember(dest => dest.Roles,
                opt => opt.MapFrom(src => src.UserRoles.Select(ur => ur.Role.Name).ToList()))
            .ReverseMap()
            .ForMember(dest => dest.Email, opt => opt.Ignore())
            .ForMember(dest => dest.UserRoles, opt => opt.Ignore());
    }
}