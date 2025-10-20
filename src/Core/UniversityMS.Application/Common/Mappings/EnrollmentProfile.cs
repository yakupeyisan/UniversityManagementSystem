using AutoMapper;
using UniversityMS.Application.Features.EnrollmentFeature.DTOs;
using UniversityMS.Domain.Entities.EnrollmentAggregate;

namespace UniversityMS.Application.Common.Mappings;

public class EnrollmentProfile : Profile
{
    public EnrollmentProfile()
    {
        // ============= ENROLLMENT MAPPING =============
        CreateMap<Enrollment, EnrollmentDto>()
            .ForMember(dest => dest.StudentId,
                opt => opt.MapFrom(src => src.StudentId))
            .ForMember(dest => dest.Status,
                opt => opt.MapFrom(src => src.Status))
            .ForMember(dest => dest.EnrollmentDate,
                opt => opt.MapFrom(src => src.CreatedAt))
            .ReverseMap()
            .ForMember(dest => dest.Status, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore());

        // ============= COURSE REGISTRATION MAPPING =============
        // NOT: CourseRegistrationDto.Status string olarak işleniyor, CourseRegistrationStatus enum değeri kullanmakla ilgiliyse kontrol gerekir
        CreateMap<CourseRegistration, CourseRegistrationDto>()
            .ForMember(dest => dest.CourseName,
                opt => opt.MapFrom(src => src.Course.Name))
            .ForMember(dest => dest.CourseCode,
                opt => opt.MapFrom(src => src.Course.Code))
            .ForMember(dest => dest.Credits,
                opt => opt.MapFrom(src => src.ECTS))
            .ForMember(dest => dest.Status,
                opt => opt.MapFrom(src => src.Status.ToString()))
            .ForMember(dest => dest.RegistrationDate,
                opt => opt.MapFrom(src => src.CreatedAt))
            .ReverseMap()
            .ForMember(dest => dest.Course, opt => opt.Ignore())
            .ForMember(dest => dest.Status, opt => opt.Ignore());
    }
}