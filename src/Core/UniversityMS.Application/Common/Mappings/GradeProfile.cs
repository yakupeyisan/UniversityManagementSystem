using AutoMapper;
using UniversityMS.Application.Features.GradeFeature.DTOs;
using UniversityMS.Domain.Entities.EnrollmentAggregate;

namespace UniversityMS.Application.Common.Mappings;

public class GradeProfile : Profile
{
    public GradeProfile()
    {
        // ============= GRADE DETAIL DTO =============
        CreateMap<Grade, GradeDetailDto>()
            .ForMember(dest => dest.CourseName,
                opt => opt.MapFrom(src => src.CourseRegistration.Course.Name))
            .ForMember(dest => dest.StudentId,
                opt => opt.MapFrom(src => src.CourseRegistration.Enrollment.StudentId))
            .ForMember(dest => dest.CourseId,
                opt => opt.MapFrom(src => src.CourseRegistration.CourseId))
            .ForMember(dest => dest.Credits,
                opt => opt.MapFrom(src => src.CourseRegistration.ECTS))
            .ForMember(dest => dest.LetterGrade,
                opt => opt.MapFrom(src => src.LetterGrade.ToString()))
            .ReverseMap()
            .ForMember(dest => dest.CourseRegistration, opt => opt.Ignore())
            .ForMember(dest => dest.LetterGrade, opt => opt.Ignore());

        // ============= TRANSCRIPT COURSE DTO =============
        CreateMap<Grade, TranscriptCourseDto>()
            .ForMember(dest => dest.CourseCode,
                opt => opt.MapFrom(src => src.CourseRegistration.Course.Code))
            .ForMember(dest => dest.CourseName,
                opt => opt.MapFrom(src => src.CourseRegistration.Course.Name))
            .ForMember(dest => dest.Credits,
                opt => opt.MapFrom(src => src.CourseRegistration.ECTS))
            .ForMember(dest => dest.CourseId,
                opt => opt.MapFrom(src => src.CourseRegistration.CourseId))
            .ForMember(dest => dest.AcademicYear,
                opt => opt.MapFrom(src => src.CourseRegistration.Enrollment.AcademicYear))
            .ForMember(dest => dest.Semester,
                opt => opt.MapFrom(src => src.CourseRegistration.Enrollment.Semester))
            .ForMember(dest => dest.LetterGrade,
                opt => opt.MapFrom(src => src.LetterGrade.ToString()))
            .ReverseMap()
            .ForMember(dest => dest.CourseRegistration, opt => opt.Ignore())
            .ForMember(dest => dest.LetterGrade, opt => opt.Ignore());

        // ============= GRADE OBJECTION DTO =============
        CreateMap<GradeObjection, GradeObjectionDto>()
            .ForMember(dest => dest.Status,
                opt => opt.MapFrom(src => src.Status.ToString()))
            .ForMember(dest => dest.Description,
                opt => opt.MapFrom(src => src.Reason))
            .ForMember(dest => dest.CreatedAt,
                opt => opt.MapFrom(src => src.CreatedAt))
            .ForMember(dest => dest.ReviewedAt,
                opt => opt.MapFrom(src => src.ReviewDate))
            .ReverseMap()
            .ForMember(dest => dest.Status, opt => opt.Ignore())
            .ForMember(dest => dest.Grade, opt => opt.Ignore())
            .ForMember(dest => dest.Reason, opt => opt.MapFrom(src => src.Description));
    }
}