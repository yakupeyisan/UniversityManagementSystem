using AutoMapper;
using UniversityMS.Application.Features.AttendanceFeature.DTOs;
using UniversityMS.Domain.Entities.EnrollmentAggregate;

namespace UniversityMS.Application.Common.Mappings;

public class AttendanceProfile : Profile
{
    public AttendanceProfile()
    {
        // ============= ATTENDANCE RECORD DTO =============
        CreateMap<Attendance, AttendanceRecordDto>()
            .ForMember(dest => dest.Id,
                opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.WeekNumber,
                opt => opt.MapFrom(src => src.WeekNumber))
            .ForMember(dest => dest.IsPresent,
                opt => opt.MapFrom(src => src.IsPresent))
            .ForMember(dest => dest.RecordedAt,
                opt => opt.MapFrom(src => src.CreatedAt))
            .ForMember(dest => dest.QRCode,
                opt => opt.MapFrom(src => src.Method == Domain.Enums.AttendanceMethod.QRCode
                    ? "QR Code"
                    : src.Method.ToString()))
            .ReverseMap()
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.Method, opt => opt.Ignore());

        // ============= STUDENT ATTENDANCE DTO =============
        CreateMap<Attendance, StudentAttendanceDto>()
            .IncludeMembers(src => src)
            .ReverseMap();
    }
}