using MediatR;
using UniversityMS.Application.Common.Models;
using UniversityMS.Application.Features.PayrollFeature.DTOs;

namespace UniversityMS.Application.Features.PayrollFeature.Queries;

/// <summary>
/// Bordroyu ID'ye göre getir
/// </summary>
public record GetPayrollByIdQuery(Guid PayrollId) : IRequest<Result<PayrollDto>>;