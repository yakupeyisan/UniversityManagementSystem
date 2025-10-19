using UniversityMS.Domain.Entities.HRAggregate;
using UniversityMS.Domain.Entities.PayrollAggregate;

namespace UniversityMS.Application.Common.Interfaces;

public interface IPayslipGenerationService
{
    Task<Payslip> GeneratePayslipAsync(Payroll payroll, Employee employee, CancellationToken cancellationToken);
    Task<string> GeneratePdfAsync(Payroll payroll, Employee employee, CancellationToken cancellationToken);
    Task<bool> SendPayslipEmailAsync(Payslip payslip, Employee employee, CancellationToken cancellationToken);
}
