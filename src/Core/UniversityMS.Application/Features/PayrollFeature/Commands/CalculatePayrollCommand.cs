using MediatR;
using UniversityMS.Application.Common.Models;
using UniversityMS.Application.Features.PayrollFeature.DTOs;

namespace UniversityMS.Application.Features.PayrollFeature.Commands;

/// <summary>
/// Bordroyu hesapla - Taslaktan Hesaplanmış durumuna geç
/// </summary>
public record CalculatePayrollCommand(Guid PayrollId) : IRequest<Result<PayrollDto>>;