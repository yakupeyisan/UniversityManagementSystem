using MediatR;
using UniversityMS.Application.Common.Models;

namespace UniversityMS.Application.Features.PayrollFeature.Commands;

/// <summary>
/// Bordroyu sil - Sadece taslak durumundaki bordro silinebilir
/// </summary>
public record DeletePayrollCommand(Guid PayrollId, string Reason) : IRequest<Result<bool>>;