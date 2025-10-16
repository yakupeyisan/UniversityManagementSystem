using MediatR;
using UniversityMS.Application.Common.Models;
using UniversityMS.Application.Features.PayrollFeature.DTOs;

namespace UniversityMS.Application.Features.PayrollFeature.Commands;

public record CreatePayrollCommand(
    Guid EmployeeId,
    int Month,
    int Year,
    decimal BaseSalary,
    decimal? Bonus,
    List<PayrollDeductionDto> Deductions
) : IRequest<Result<PayrollDto>>;