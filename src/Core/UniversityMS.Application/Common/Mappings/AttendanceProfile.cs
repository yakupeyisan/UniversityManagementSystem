using AutoMapper;
using UniversityMS.Application.Features.AttendanceFeature.DTOs;
using UniversityMS.Domain.Entities.EnrollmentAggregate;

namespace UniversityMS.Application.Common.Mappings;

public class AttendanceProfile : Profile
{
    public AttendanceProfile()
    {
        CreateMap<Attendance, AttendanceRecordDto>()
            .ForMember(dest => dest.RecordedAt, opt => opt.MapFrom(src => src.CreatedAt))
            .ReverseMap();

        CreateMap<Attendance, StudentAttendanceDto>()
            .ReverseMap();
    }
}