using MediatR;
using UniversityMS.Application.Common.Models;
using UniversityMS.Application.Features.ProcurementFeature.DTOs;

namespace UniversityMS.Application.Features.ProcurementFeature.Queries;

public class GetPurchaseOrderListQuery : IRequest<Result<PaginatedList<PurchaseOrderDto>>>
{
    public string? Status { get; set; }
    public Guid? SupplierId { get; set; }
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 10;
}