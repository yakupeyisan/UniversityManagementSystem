using UniversityMS.Application.Common.Models;
using UniversityMS.Domain.Entities.PayrollAggregate;

namespace UniversityMS.Application.Common.Interfaces;

public interface IPayrollCalculationService
{
    Task CalculatePayrollAsync(Payroll payroll, CancellationToken cancellationToken);
    Task<PayrollCalculationResult> CalculateTaxesAsync(decimal grossSalary);
    Task<SGKContributionResult> CalculateSGKAsync(decimal grossSalary);
}