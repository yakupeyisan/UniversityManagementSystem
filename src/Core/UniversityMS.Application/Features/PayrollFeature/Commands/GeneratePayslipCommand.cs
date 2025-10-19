using MediatR;
using UniversityMS.Application.Common.Models;
using UniversityMS.Application.Features.PayrollFeature.DTOs;

namespace UniversityMS.Application.Features.PayrollFeature.Commands;

public class GeneratePayslipCommand : IRequest<Result<PayslipDto>>
{
    public Guid PayrollId { get; set; }
}