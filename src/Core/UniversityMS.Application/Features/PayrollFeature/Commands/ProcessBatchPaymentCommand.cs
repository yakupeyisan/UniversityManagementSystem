using MediatR;
using UniversityMS.Application.Common.Models;
using UniversityMS.Application.Features.PayrollFeature.DTOs;

namespace UniversityMS.Application.Features.PayrollFeature.Commands;

public record ProcessBatchPaymentCommand(
    List<Guid> PayrollIds,
    Guid? ProcessedBy = null,
    string? Notes = null
) : IRequest<Result<BatchPaymentResultDto>>;