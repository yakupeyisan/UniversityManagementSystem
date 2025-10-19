using MediatR;
using UniversityMS.Application.Common.Models;
using UniversityMS.Application.Features.Inventory.DTOs;

namespace UniversityMS.Application.Features.Inventory.Queries;

public class GetInventoryReportQuery : IRequest<Result<InventoryReportDto>>
{
    public Guid WarehouseId { get; set; }
}