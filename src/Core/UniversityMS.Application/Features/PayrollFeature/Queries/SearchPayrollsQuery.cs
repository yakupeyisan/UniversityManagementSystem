using MediatR;
using UniversityMS.Application.Common.Models;
using UniversityMS.Application.Features.PayrollFeature.DTOs;

namespace UniversityMS.Application.Features.PayrollFeature.Queries;

/// <summary>
/// Başlangıç-Bitiş tarihi aralığında bordro ara
/// </summary>
public record SearchPayrollsQuery(
    DateTime StartDate,
    DateTime EndDate,
    string? EmployeeNumber = null,
    string? Status = null,
    int PageNumber = 1,
    int PageSize = 50
) : IRequest<Result<PaginatedList<PayrollDto>>>;