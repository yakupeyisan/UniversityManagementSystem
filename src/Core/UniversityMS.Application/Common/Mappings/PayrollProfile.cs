using AutoMapper;
using UniversityMS.Application.Features.PayrollFeature.DTOs;
using UniversityMS.Domain.Entities.PayrollAggregate;

namespace UniversityMS.Application.Common.Mappings;

public class PayrollProfile : Profile
{
    public PayrollProfile()
    {
        // ============= PAYROLL DTO =============
        CreateMap<Payslip, PayrollDto>()
            .ForMember(dest => dest.ApprovedDate,
                opt => opt.MapFrom(src => src.ApprovedDate))
            .ForMember(dest => dest.Status,
                opt => opt.MapFrom(src => src.Status.ToString()))
            .ReverseMap()
            .ForMember(dest => dest.Status, opt => opt.Ignore());
    }
}