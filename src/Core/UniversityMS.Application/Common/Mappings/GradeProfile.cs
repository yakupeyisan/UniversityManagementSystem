using AutoMapper;
using UniversityMS.Application.Features.GradeFeature.DTOs;
using UniversityMS.Domain.Entities.EnrollmentAggregate;

namespace UniversityMS.Application.Common.Mappings;

public class GradeProfile : Profile
{
    public GradeProfile()
    {
        CreateMap<Grade, GradeDetailDto>()
            .ForMember(dest => dest.CourseName, opt => opt.MapFrom(src => src.CourseRegistration.Course.Name))
            .ForMember(dest => dest.StudentId, opt => opt.MapFrom(src => src.CourseRegistration.Enrollment.StudentId))
            .ForMember(dest => dest.CourseId, opt => opt.MapFrom(src => src.CourseRegistration.CourseId))
            // ECTS olarak gönder, DTO'da Credits isminde
            .ForMember(dest => dest.Credits, opt => opt.MapFrom(src => src.CourseRegistration.ECTS))
            .ReverseMap();

        CreateMap<Grade, TranscriptCourseDto>()
            .ForMember(dest => dest.CourseCode, opt => opt.MapFrom(src => src.CourseRegistration.Course.Code))
            .ForMember(dest => dest.CourseName, opt => opt.MapFrom(src => src.CourseRegistration.Course.Name))
            // ECTS olarak gönder, DTO'da Credits isminde
            .ForMember(dest => dest.Credits, opt => opt.MapFrom(src => src.CourseRegistration.ECTS))
            .ForMember(dest => dest.CourseId, opt => opt.MapFrom(src => src.CourseRegistration.CourseId))
            .ForMember(dest => dest.AcademicYear, opt => opt.MapFrom(src => src.CourseRegistration.Enrollment.AcademicYear))
            .ForMember(dest => dest.Semester, opt => opt.MapFrom(src => src.CourseRegistration.Enrollment.Semester))
            .ReverseMap();

        CreateMap<GradeObjection, GradeObjectionDto>()
            .ReverseMap();
    }
}