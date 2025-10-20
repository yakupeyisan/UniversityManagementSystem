using MediatR;
using UniversityMS.Application.Common.Models;
using UniversityMS.Application.Features.FinanceFeature.DTOs;

namespace UniversityMS.Application.Features.FinanceFeature.Queries;

public record GetInvoiceListQuery(
    int PageNumber = 1,
    int PageSize = 10,
    string? Status = null,
    string? Type = null,
    DateTime? FromDate = null,
    DateTime? ToDate = null,
    Guid? SupplierId = null
) : IRequest<Result<PaginatedList<InvoiceDto>>>;