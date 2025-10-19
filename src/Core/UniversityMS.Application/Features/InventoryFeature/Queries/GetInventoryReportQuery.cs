using MediatR;
using UniversityMS.Application.Common.Models;
using UniversityMS.Application.Features.InventoryFeature.DTOs;

namespace UniversityMS.Application.Features.InventoryFeature.Queries;

public class GetInventoryReportQuery : IRequest<Result<InventoryReportDto>>
{
    public Guid WarehouseId { get; set; }
}