using MediatR;
using UniversityMS.Application.Common.Models;
using UniversityMS.Application.Features.PayrollFeature.DTOs;

namespace UniversityMS.Application.Features.PayrollFeature.Commands;

public record ProcessPaymentCommand(
    Guid PayrollId
) : IRequest<Result<PayrollDto>>;