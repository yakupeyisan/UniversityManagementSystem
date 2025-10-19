using AutoMapper;
using UniversityMS.Application.Features.EnrollmentFeature.DTOs;
using UniversityMS.Domain.Entities.EnrollmentAggregate;

namespace UniversityMS.Application.Common.Mappings;

public class EnrollmentProfile : Profile
{
    public EnrollmentProfile()
    {
        CreateMap<Enrollment, EnrollmentDto>()
            .ReverseMap();

        CreateMap<CourseRegistration, CourseRegistrationDto>()
            .ForMember(dest => dest.CourseName, opt => opt.MapFrom(src => src.Course.Name))
            .ForMember(dest => dest.CourseCode, opt => opt.MapFrom(src => src.Course.Code))
            .ReverseMap();
    }
}