using MediatR;
using UniversityMS.Application.Common.Models;
using UniversityMS.Application.Features.PayrollFeature.DTOs;

namespace UniversityMS.Application.Features.PayrollFeature.Commands;

/// <summary>
/// Ödeme bilgisi ile bordro ödemesi
/// Banka bilgisi, referans numarası vb. bilgiler içerebilir
/// </summary>
public record ProcessPaymentCommandWithDetails(
    Guid PayrollId,
    Guid? PaidBy = null,
    string? BankName = null,
    string? AccountNumber = null,
    string? TransactionReference = null,
    string? Notes = null
) : IRequest<Result<PayrollDto>>;