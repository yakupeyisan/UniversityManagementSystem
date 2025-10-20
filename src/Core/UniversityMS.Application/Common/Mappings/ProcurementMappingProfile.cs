using AutoMapper;
using UniversityMS.Application.Features.ProcurementFeature.DTOs;
using UniversityMS.Domain.Entities.ProcurementAggregate;

namespace UniversityMS.Application.Common.Mappings;

public class ProcurementMappingProfile : Profile
{
    public ProcurementMappingProfile()
    {
        CreateMap<PurchaseRequest, PurchaseRequestDto>()
            .ReverseMap();

        CreateMap<PurchaseItem, PurchaseItemDto>()
            .ReverseMap();
    }
}